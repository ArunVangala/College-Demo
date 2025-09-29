
// Models/Course.cs
using System.ComponentModel.DataAnnotations;

namespace SriSaiCollegeManagement.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, 8)]
        public int Duration { get; set; } // in semesters

        [Required]
        [StringLength(50)]
        public string Department { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
    }
}