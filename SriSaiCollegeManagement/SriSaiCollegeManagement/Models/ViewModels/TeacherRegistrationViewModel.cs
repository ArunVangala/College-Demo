
// Models/ViewModels/TeacherRegistrationViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace SriSaiCollegeManagement.Models.ViewModels
{
    public class TeacherRegistrationViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Qualification")]
        public string Qualification { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; }

        [Required]
        [Range(0, 50)]
        [Display(Name = "Years of Experience")]
        public int Experience { get; set; }

        [Required]
        [Range(10000, 200000)]
        [Display(Name = "Salary")]
        public decimal Salary { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}