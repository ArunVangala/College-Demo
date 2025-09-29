
// Models/ViewModels/TeacherDashboardViewModel.cs
namespace SriSaiCollegeManagement.Models.ViewModels
{
    public class TeacherDashboardViewModel
    {
        public Teacher Teacher { get; set; }
        public List<Subject> AssignedSubjects { get; set; } = new List<Subject>();
        public List<Exam> ScheduledExams { get; set; } = new List<Exam>();
        public int TotalStudents { get; set; }
        public List<Announcement> Announcements { get; set; } = new List<Announcement>();
    }
}