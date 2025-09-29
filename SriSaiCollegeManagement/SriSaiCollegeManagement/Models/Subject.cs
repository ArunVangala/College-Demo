
// Models/Subject.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SriSaiCollegeManagement.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string SubjectCode { get; set; }

        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; }

        [Required]
        public int Credits { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int Semester { get; set; }

        public int? TeacherId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher? Teacher { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
    }
}
