
// Models/ViewModels/StudentReportViewModel.cs
namespace SriSaiCollegeManagement.Models.ViewModels
{
    public class StudentReportViewModel
    {
        public Student Student { get; set; }
        public List<SubjectWiseReport> SubjectReports { get; set; } = new List<SubjectWiseReport>();
        public double OverallPercentage { get; set; }
        public double OverallAttendance { get; set; }
        public string OverallGrade { get; set; }
        public int CurrentSemester { get; set; }
    }

    public class SubjectWiseReport
    {
        public Subject Subject { get; set; }
        public List<Result> Results { get; set; } = new List<Result>();
        public double AttendancePercentage { get; set; }
        public double AverageMarks { get; set; }
        public string Grade { get; set; }
    }
}