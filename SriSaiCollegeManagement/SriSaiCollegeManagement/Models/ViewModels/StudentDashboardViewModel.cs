
// Models/ViewModels/StudentDashboardViewModel.cs
namespace SriSaiCollegeManagement.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public Student Student { get; set; }
        public List<Subject> EnrolledSubjects { get; set; } = new List<Subject>();
        public List<Exam> UpcomingExams { get; set; } = new List<Exam>();
        public List<Result> RecentResults { get; set; } = new List<Result>();
        public List<Announcement> Announcements { get; set; } = new List<Announcement>();
        public double OverallAttendance { get; set; }
        public string CurrentSemester { get; set; }
    }
}