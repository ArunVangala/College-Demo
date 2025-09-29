// Controllers/HomeController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SriSaiCollegeManagement.Data;
using SriSaiCollegeManagement.Models;
using SriSaiCollegeManagement.Models.ViewModels;
using System.Diagnostics;

namespace SriSaiCollegeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalStudents = await _context.Students.CountAsync(s => s.IsActive),
                TotalTeachers = await _context.Teachers.CountAsync(t => t.IsActive),
                TotalCourses = await _context.Courses.CountAsync(c => c.IsActive),
                TotalSubjects = await _context.Subjects.CountAsync(s => s.IsActive),
                RecentAnnouncements = await _context.Announcements
                    .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > DateTime.Now))
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

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public async Task<IActionResult> Announcements()
        {
            var announcements = await _context.Announcements
                .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > DateTime.Now))
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();

            return View(announcements);
        }

        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses
                .Where(c => c.IsActive)
                .ToListAsync();

            return View(courses);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
