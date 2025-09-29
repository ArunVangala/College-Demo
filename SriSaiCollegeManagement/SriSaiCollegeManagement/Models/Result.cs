
// Models/Result.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SriSaiCollegeManagement.Models
{
    public class Result
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int ExamId { get; set; }

        [Required]
        [Range(0, 1000)]
        public int MarksObtained { get; set; }

        public string? Grade { get; set; }

        public bool IsPassed { get; set; }

        public DateTime ResultDate { get; set; }

        public string? Remarks { get; set; }

        // Navigation Properties
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        [NotMapped]
        public double Percentage => (double)MarksObtained / Exam.TotalMarks * 100;
    }
}