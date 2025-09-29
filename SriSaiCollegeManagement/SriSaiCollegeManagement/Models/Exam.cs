// Models/Exam.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SriSaiCollegeManagement.Models
{
    public class Exam
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ExamName { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public DateTime ExamDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Range(1, 1000)]
        public int TotalMarks { get; set; }

        [Required]
        [Range(1, 1000)]
        public int PassMarks { get; set; }

        [Required]
        [StringLength(50)]
        public string ExamType { get; set; } // Internal, Mid-term, Final, etc.

        public string? Instructions { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }

        public virtual ICollection<Result> Results { get; set; }
    }
}