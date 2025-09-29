
// Controllers/TeacherController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SriSaiCollegeManagement.Data;
using SriSaiCollegeManagement.Models;
using SriSaiCollegeManagement.Models.ViewModels;

namespace SriSaiCollegeManagement.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TeacherController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email == user.Email);

            if (teacher == null)
            {
                return RedirectToAction("Profile");
            }

            var assignedSubjects = await _context.Subjects
                .Include(s => s.Course)
                .Where(s => s.TeacherId == teacher.Id && s.IsActive)
                .ToListAsync();

            var scheduledExams = await _context.Exams
                .Include(e => e.Subject)
                .Where(e => e.IsActive &&
                           assignedSubjects.Select(s => s.Id).Contains(e.SubjectId))
                .OrderBy(e => e.ExamDate)
                .Take(5)
                .ToListAsync();

            var totalStudents = await _context.Enrollments
                .Where(e => assignedSubjects.Select(s => s.Id).Contains(e.SubjectId) && e.IsActive)
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync();

            var announcements = await _context.Announcements
                .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > DateTime.Now))
                .OrderByDescending(a => a.CreatedDate)
                .Take(5)
                .ToListAsync();

            var viewModel = new TeacherDashboardViewModel
            {
                Teacher = teacher,
                AssignedSubjects = assignedSubjects,
                ScheduledExams = scheduledExams,
                TotalStudents = totalStudents,
                Announcements = announcements
            };

            return View(viewModel);
        }

        public async Task<IActionResult> MySubjects()
        {
            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email == user.Email);

            if (teacher == null)
            {
                return RedirectToAction("Profile");
            }

            var subjects = await _context.Subjects
                .Include(s => s.Course)
                .Where(s => s.TeacherId == teacher.Id && s.IsActive)
                .ToListAsync();

            return View(subjects);
        }

        [HttpGet]
        public async Task<IActionResult> MarkAttendance()
        {
            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email == user.Email);

            if (teacher == null)
            {
                return RedirectToAction("Profile");
            }

            var subjects = await _context.Subjects
                .Where(s => s.TeacherId == teacher.Id && s.IsActive)
                .ToListAsync();

            var viewModel = new AttendanceViewModel
            {
                Date = DateTime.Today,
                AvailableSubjects = subjects
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAttendance(AttendanceViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Remove existing attendance for the same date and subject
                var existingAttendances = await _context.Attendances
                    .Where(a => a.SubjectId == model.SubjectId && a.Date.Date == model.Date.Date)
                    .ToListAsync();

                _context.Attendances.RemoveRange(existingAttendances);

                // Add new attendance records
                foreach (var studentAttendance in model.StudentAttendances)
                {
                    var attendance = new Attendance
                    {
                        StudentId = studentAttendance.StudentId,
                        SubjectId = model.SubjectId,
                        Date = model.Date,
                        IsPresent = studentAttendance.IsPresent,
                        Remarks = studentAttendance.Remarks
                    };
                    _context.Attendances.Add(attendance);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Attendance marked successfully!";
                return RedirectToAction("MarkAttendance");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentsForSubject(int subjectId, DateTime date)
        {
            var students = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.SubjectId == subjectId && e.IsActive)
                .Select(e => e.Student)
                .ToListAsync();

            // Check if attendance already exists for this date
            var existingAttendances = await _context.Attendances
                .Where(a => a.SubjectId == subjectId && a.Date.Date == date.Date)
                .ToListAsync();

            var studentAttendances = students.Select(s => new StudentAttendanceEntry
            {
                StudentId = s.Id,
                StudentName = s.FullName,
                StudentId_Display = s.StudentId,
                IsPresent = existingAttendances.FirstOrDefault(a => a.StudentId == s.Id)?.IsPresent ?? false,
                Remarks = existingAttendances.FirstOrDefault(a => a.StudentId == s.Id)?.Remarks,
                HasExistingAttendance = existingAttendances.Any(a => a.StudentId == s.Id)
            }).ToList();

            return Json(studentAttendances);
        }

        [HttpGet]
        public async Task<IActionResult> EnterResults()
        {
            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email == user.Email);

            if (teacher == null)
            {
                return RedirectToAction("Profile");
            }

            var exams = await _context.Exams
                .Include(e => e.Subject)
                .Where(e => e.Subject.TeacherId == teacher.Id && e.IsActive)
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync();

            return View(exams);
        }

        [HttpGet]
        public async Task<IActionResult> EnterExamResults(int examId)
        {
            var exam = await _context.Exams
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
            {
                return NotFound();
            }

            var students = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.SubjectId == exam.SubjectId && e.IsActive)
                .Select(e => e.Student)
                .ToListAsync();

            var existingResults = await _context.Results
                .Where(r => r.ExamId == examId)
                .ToListAsync();

            var studentResults = students.Select(s =>
            {
                var existingResult = existingResults.FirstOrDefault(r => r.StudentId == s.Id);
                return new StudentResultEntry
                {
                    StudentId = s.Id,
                    StudentName = s.FullName,
                    StudentId_Display = s.StudentId,
                    MarksObtained = existingResult?.MarksObtained ?? 0,
                    Grade = existingResult?.Grade,
                    Remarks = existingResult?.Remarks,
                    IsPassed = existingResult?.IsPassed ?? false,
                    HasExistingResult = existingResult != null,
                    ExistingResultId = existingResult?.Id
                };
            }).ToList();

            var viewModel = new ResultEntryViewModel
            {
                ExamId = examId,
                Exam = exam,
                StudentResults = studentResults
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EnterExamResults(ResultEntryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var exam = await _context.Exams.FindAsync(model.ExamId);
                if (exam == null) return NotFound();

                foreach (var studentResult in model.StudentResults)
                {
                    // Calculate grade and pass status
                    var percentage = (double)studentResult.MarksObtained / exam.TotalMarks * 100;
                    var grade = GetGrade(percentage);
                    var isPassed = studentResult.MarksObtained >= exam.PassMarks;

                    if (studentResult.HasExistingResult && studentResult.ExistingResultId.HasValue)
                    {
                        // Update existing result
                        var existingResult = await _context.Results.FindAsync(studentResult.ExistingResultId.Value);
                        if (existingResult != null)
                        {
                            existingResult.MarksObtained = studentResult.MarksObtained;
                            existingResult.Grade = grade;
                            existingResult.IsPassed = isPassed;
                            existingResult.Remarks = studentResult.Remarks;
                            existingResult.ResultDate = DateTime.Now;
                        }
                    }
                    else
                    {
                        // Create new result
                        var result = new Result
                        {
                            StudentId = studentResult.StudentId,
                            ExamId = model.ExamId,
                            MarksObtained = studentResult.MarksObtained,
                            Grade = grade,
                            IsPassed = isPassed,
                            Remarks = studentResult.Remarks,
                            ResultDate = DateTime.Now
                        };
                        _context.Results.Add(result);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Results entered successfully!";
                return RedirectToAction("EnterResults");
            }

            return View(model);
        }

        private string GetGrade(double percentage)
        {
            if (percentage >= 90) return "A+";
            if (percentage >= 80) return "A";
            if (percentage >= 70) return "B+";
            if (percentage >= 60) return "B";
            if (percentage >= 50) return "C+";
            if (percentage >= 40) return "C";
            return "F";
        }
    }
}

