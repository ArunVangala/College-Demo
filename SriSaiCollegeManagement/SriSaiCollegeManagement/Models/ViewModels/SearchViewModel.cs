
// Models/ViewModels/SearchViewModel.cs
namespace SriSaiCollegeManagement.Models.ViewModels
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }
        public string SearchType { get; set; } // Students, Teachers, Courses, etc.
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
    }
}