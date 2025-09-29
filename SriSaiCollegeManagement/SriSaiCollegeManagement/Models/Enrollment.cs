// Models/Enrollment.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace SriSaiCollegeManagement.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int SubjectId { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
    }
}