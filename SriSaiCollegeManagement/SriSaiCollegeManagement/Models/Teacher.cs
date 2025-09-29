
// Models/Teacher.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SriSaiCollegeManagement.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string TeacherId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [StringLength(100)]
        public string Qualification { get; set; }

        [Required]
        [StringLength(100)]
        public string Department { get; set; }

        [Required]
        public DateTime JoiningDate { get; set; }

        [Required]
        [Range(0, 50)]
        public int Experience { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Salary { get; set; }

        public string? ProfilePicture { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<Subject> Subjects { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}