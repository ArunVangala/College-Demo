// Models/Student.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SriSaiCollegeManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string StudentId { get; set; }

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
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int Semester { get; set; }

        public DateTime AdmissionDate { get; set; }

        public string? ProfilePicture { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}