
// Models/ViewModels/ResultEntryViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace SriSaiCollegeManagement.Models.ViewModels
{
    public class ResultEntryViewModel
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public List<StudentResultEntry> StudentResults { get; set; } = new List<StudentResultEntry>();
    }

    public class StudentResultEntry
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentId_Display { get; set; }

        [Required]
        [Range(0, 1000)]
        [Display(Name = "Marks Obtained")]
        public int MarksObtained { get; set; }

        [Display(Name = "Grade")]
        public string? Grade { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        public bool IsPassed { get; set; }
        public bool HasExistingResult { get; set; }
        public int? ExistingResultId { get; set; }
    }
}
