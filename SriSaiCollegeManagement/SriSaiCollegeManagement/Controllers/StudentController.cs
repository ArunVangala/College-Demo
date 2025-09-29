
// Controllers/StudentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SriSaiCollegeManagement.Data;
using SriSaiCollegeManagement.Models;
using SriSaiCollegeManagement.Models.ViewModels;

namespace SriSaiCollegeManagement.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Email == user.Email);

            if (student == null)
            {
                return RedirectToAction("Profile", "Student");
            }

            var enrolledSubjects = await _context.Enrollments
                .Include(e => e.Subject)
                .ThenInclude(s => s.Teacher)
                .Where(e => e.StudentId == student.Id && e.IsActive)
                .Select(e => e.Subject)
                .ToListAsync();

            var upcomingExams = await _context.Exams
                .Include(e => e.Subject)
                .Where(e => e.IsActive &&
                           e.ExamDate > DateTime.Now &&
                           enrolledSubjects.Select(s => s.Id).Contains(e.SubjectId))
                .OrderBy(e => e.ExamDate)
                .Take(5)
                .ToListAsync();

            var recentResults = await _context.Results
                .Include(r => r.Exam)
                .ThenInclude(e => e.Subject)
                .Where(r => r.StudentId == student.Id)
                .OrderByDescending(r => r.ResultDate)
                .Take(5)
                .ToListAsync();

            var totalClasses = await _context.Attendances
                .Where(a => a.StudentId == student.Id)
                .CountAsync();

            var presentClasses = await _context.Attendances
                .Where(a => a.StudentId == student.Id && a.IsPresent)
                .CountAsync();

            var overallAttendance = totalClasses > 0 ? (double)presentClasses / totalClasses * 100 : 0;

            var announcements = await _context.Announcements
                .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > DateTime.Now))
                .OrderByDescending(a => a.CreatedDate)
                .Take(5)
                .ToListAsync();

            var viewModel = new StudentDashboardViewModel
            {
                Student = student,
                EnrolledSubjects = enrolledSubjects,
                UpcomingExams = upcomingExams,
                RecentResults = recentResults,
                OverallAttendance = overallAttendance,
                Announcements = announcements,
                CurrentSemester = $"Semester {student.Semester}"
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Email == user.Email);

            if (student == null)
            {
                // Redirect to create profile if student record doesn't exist
                return RedirectToAction("CreateProfile");
            }

            return View(student);
        }

        public async Task<IActionResult> Results()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == user.Email);

            if (student == null)
            {
                return RedirectToAction("Profile");
            }

            var results = await _context.Results
                .Include(r => r.Exam)
                .ThenInclude(e => e.Subject)
                .Where(r => r.StudentId == student.Id)
                .OrderByDescending(r => r.ResultDate)
                .ToListAsync();

            return View(results);
        }

        public async Task<IActionResult> Attendance()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == user.Email);

            if (student == null)
            {
                return RedirectToAction("Profile");
            }

            var attendances = await _context.Attendances
                .Include(a => a.Subject)
                .Where(a => a.StudentId == student.Id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            // Group by subject for better display
            var attendanceBySubject = attendances
                .GroupBy(a => a.Subject)
                .Select(g => new
                {
                    Subject = g.Key,
                    TotalClasses = g.Count(),
                    PresentClasses = g.Count(a => a.IsPresent),
                    AttendancePercentage = g.Count() > 0 ? (double)g.Count(a => a.IsPresent) / g.Count() * 100 : 0,
                    Attendances = g.OrderByDescending(a => a.Date).ToList()
                })
                .ToList();

            return View(attendanceBySubject);
        }

        public async Task<IActionResult> Exams()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == user.Email);

            if (student == null)
            {
                return RedirectToAction("Profile");
            }

            var enrolledSubjectIds = await _context.Enrollments
                .Where(e => e.StudentId == student.Id && e.IsActive)
                .Select(e => e.SubjectId)
                .ToListAsync();

            var exams = await _context.Exams
                .Include(e => e.Subject)
                .Where(e => e.IsActive && enrolledSubjectIds.Contains(e.SubjectId))
                .OrderBy(e => e.ExamDate)
                .ToListAsync();

            return View(exams);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProfile()
        {
            var courses = await _context.Courses
                .Where(c => c.IsActive)
                .ToListAsync();

            var viewModel = new StudentRegistrationViewModel
            {
                AvailableCourses = courses
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile(StudentRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                // Generate Student ID
                var lastStudent = await _context.Students
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefaultAsync();

                var studentId = $"SSC{DateTime.Now.Year}{(lastStudent?.Id + 1 ?? 1):D4}";

                var student = new Student
                {
                    StudentId = studentId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = user.Email,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Address = model.Address,
                    CourseId = model.CourseId,
                    Semester = model.Semester,
                    AdmissionDate = DateTime.Now,
                    IsActive = true
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                // Auto-enroll in semester subjects
                var subjects = await _context.Subjects
                    .Where(s => s.CourseId == model.CourseId && s.Semester == model.Semester && s.IsActive)
                    .ToListAsync();

                foreach (var subject in subjects)
                {
                    var enrollment = new Enrollment
                    {
                        StudentId = student.Id,
                        SubjectId = subject.Id,
                        EnrollmentDate = DateTime.Now,
                        IsActive = true
                    };
                    _context.Enrollments.Add(enrollment);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Dashboard");
            }

            model.AvailableCourses = await _context.Courses
                .Where(c => c.IsActive)
                .ToListAsync();

            return View(model);
        }
    }
}
