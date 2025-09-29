// Models/ViewModels/DashboardViewModel.cs
using SriSaiCollegeManagement.Models;

namespace SriSaiCollegeManagement.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalSubjects { get; set; }
        public List<Announcement> RecentAnnouncements { get; set; } = new List<Announcement>();
        public List<Exam> UpcomingExams { get; set; } = new List<Exam>();
    }
}
