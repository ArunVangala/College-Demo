// Controllers/AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SriSaiCollegeManagement.Data;
using SriSaiCollegeManagement.Models;
using SriSaiCollegeManagement.Models.ViewModels;

namespace SriSaiCollegeManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new DashboardViewModel
            {
                TotalStudents = await _context.Students.CountAsync(s => s.IsActive),
                TotalTeachers = await _context.Teachers.CountAsync(t => t.IsActive),
                TotalCourses = await _context.Courses.CountAsync(c => c.IsActive),
                TotalSubjects = await _context.Subjects.CountAsync(s => s.IsActive),
                RecentAnnouncements = await _context.Announcements
                    .Where(a => a.IsActive)
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(5)
                    .ToListAsync(),
                UpcomingExams = await _context.Exams
                    .Include(e => e.Subject)
                    .Where(e => e.IsActive && e.ExamDate > DateTime.Now)
                    .OrderBy(e => e.ExamDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // Student Management
        public async Task<IActionResult> Students()
        {
            var students = await _context.Students
                .Include(s => s.Course)
                .OrderBy(s => s.StudentId)
                .ToListAsync();

            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> CreateStudent()
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
        public async Task<IActionResult> CreateStudent(StudentRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create Identity User
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");

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
                        Email = model.Email,
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
                    TempData["Success"] = "Student created successfully!";
                    return RedirectToAction("Students");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.AvailableCourses = await _context.Courses
                .Where(c => c.IsActive)
                .ToListAsync();

            return View(model);
        }

        // Teacher Management
        public async Task<IActionResult> Teachers()
        {
            var teachers = await _context.Teachers
                .OrderBy(t => t.TeacherId)
                .ToListAsync();

            return View(teachers);
        }

        [HttpGet]
        public IActionResult CreateTeacher()
        {
            return View(new TeacherRegistrationViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(TeacherRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create Identity User
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Teacher");

                    // Generate Teacher ID
                    var lastTeacher = await _context.Teachers
                        .OrderByDescending(t => t.Id)
                        .FirstOrDefaultAsync();

                    var teacherId = $"T{(lastTeacher?.Id + 1 ?? 1):D3}";

                    var teacher = new Teacher
                    {
                        TeacherId = teacherId,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Qualification = model.Qualification,
                        Department = model.Department,
                        JoiningDate = model.JoiningDate,
                        Experience = model.Experience,
                        Salary = model.Salary,
                        IsActive = true
                    };

                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Teacher created successfully!";
                    return RedirectToAction("Teachers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // Course Management
        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses
                .Include(c => c.Students)
                .Include(c => c.Subjects)
                .ToListAsync();

            return View(courses);
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(Course model)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course created successfully!";
                return RedirectToAction("Courses");
            }

            return View(model);
        }

        // Subject Management
        public async Task<IActionResult> Subjects()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Course)
                .Include(s => s.Teacher)
                .ToListAsync();

            return View(subjects);
        }

        [HttpGet]
        public async Task<IActionResult> CreateSubject()
        {
            ViewBag.Courses = await _context.Courses.Where(c => c.IsActive).ToListAsync();
            ViewBag.Teachers = await _context.Teachers.Where(t => t.IsActive).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject(Subject model)
        {
            if (ModelState.IsValid)
            {
                _context.Subjects.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Subject created successfully!";
                return RedirectToAction("Subjects");
            }

            ViewBag.Courses = await _context.Courses.Where(c => c.IsActive).ToListAsync();
            ViewBag.Teachers = await _context.Teachers.Where(t => t.IsActive).ToListAsync();
            return View(model);
        }

        // Exam Management
        public async Task<IActionResult> Exams()
        {
            var exams = await _context.Exams
                .Include(e => e.Subject)
                .ThenInclude(s => s.Course)
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync();

            return View(exams);
        }

        [HttpGet]
        public async Task<IActionResult> CreateExam()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Course)
                .Where(s => s.IsActive)
                .ToListAsync();

            var viewModel = new ExamCreateViewModel
            {
                ExamDate = DateTime.Today.AddDays(7),
                AvailableSubjects = subjects
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExam(ExamCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var exam = new Exam
                {
                    ExamName = model.ExamName,
                    SubjectId = model.SubjectId,
                    ExamDate = model.ExamDate,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    TotalMarks = model.TotalMarks,
                    PassMarks = model.PassMarks,
                    ExamType = model.ExamType,
                    Instructions = model.Instructions,
                    IsActive = true
                };

                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Exam created successfully!";
                return RedirectToAction("Exams");
            }

            model.AvailableSubjects = await _context.Subjects
                .Include(s => s.Course)
                .Where(s => s.IsActive)
                .ToListAsync();

            return View(model);
        }

        // Announcement Management
        public async Task<IActionResult> Announcements()
        {
            var announcements = await _context.Announcements
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();

            return View(announcements);
        }

        [HttpGet]
        public IActionResult CreateAnnouncement()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement(Announcement model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name;
                model.IsActive = true;

                _context.Announcements.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Announcement created successfully!";
                return RedirectToAction("Announcements");
            }

            return View(model);
        }

        // Reports
        public async Task<IActionResult> Reports()
        {
            var studentsCount = await _context.Students.CountAsync(s => s.IsActive);
            var teachersCount = await _context.Teachers.CountAsync(t => t.IsActive);
            var coursesCount = await _context.Courses.CountAsync(c => c.IsActive);
            var subjectsCount = await _context.Subjects.CountAsync(s => s.IsActive);

            ViewBag.StudentsCount = studentsCount;
            ViewBag.TeachersCount = teachersCount;
            ViewBag.CoursesCount = coursesCount;
            ViewBag.SubjectsCount = subjectsCount;

            return View();
        }
    }
}