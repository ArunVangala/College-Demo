
// Models/ViewModels/ExamCreateViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace SriSaiCollegeManagement.Models.ViewModels
{
    public class ExamCreateViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Exam Name")]
        public string ExamName { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Exam Date")]
        public DateTime ExamDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Total Marks")]
        public int TotalMarks { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Pass Marks")]
        public int PassMarks { get; set; }

        [Required]
        [Display(Name = "Exam Type")]
        public string ExamType { get; set; }

        [Display(Name = "Instructions")]
        public string? Instructions { get; set; }

        public List<Subject> AvailableSubjects { get; set; } = new List<Subject>();
    }
}
