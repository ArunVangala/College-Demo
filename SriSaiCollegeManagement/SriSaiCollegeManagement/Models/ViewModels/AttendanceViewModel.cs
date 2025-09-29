
// Models/ViewModels/AttendanceViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace SriSaiCollegeManagement.Models.ViewModels
{
    public class AttendanceViewModel
    {
        [Required]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        public Subject Subject { get; set; }
        public List<StudentAttendanceEntry> StudentAttendances { get; set; } = new List<StudentAttendanceEntry>();
        public List<Subject> AvailableSubjects { get; set; } = new List<Subject>();
    }

    public class StudentAttendanceEntry
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentId_Display { get; set; }
        public bool IsPresent { get; set; }
        public string? Remarks { get; set; }
        public bool HasExistingAttendance { get; set; }
    }
}