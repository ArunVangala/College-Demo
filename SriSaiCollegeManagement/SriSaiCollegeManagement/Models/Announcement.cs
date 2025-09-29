
// Models/Announcement.cs
using System.ComponentModel.DataAnnotations;

namespace SriSaiCollegeManagement.Models
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Priority { get; set; } // High, Medium, Low

        [Required]
        [StringLength(50)]
        public string Category { get; set; } // Academic, General, Exam, etc.

        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
    }
}