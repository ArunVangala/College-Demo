using System.ComponentModel.DataAnnotations.Schema;

namespace SriSaiCollegeManagement.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int SubjectId { get; set; }

        public DateTime Date { get; set; }

        public bool IsPresent { get; set; }

        public string? Remarks { get; set; }

        // Navigation Properties
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
    }
}