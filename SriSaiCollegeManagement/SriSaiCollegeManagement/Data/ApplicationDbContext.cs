// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SriSaiCollegeManagement.Data;
using SriSaiCollegeManagement.Models;

namespace SriSaiCollegeManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships and constraints

            // Student-Course relationship
            builder.Entity<Student>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Subject-Course relationship
            builder.Entity<Subject>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Subjects)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Subject-Teacher relationship
            builder.Entity<Subject>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Subjects)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Enrollment relationships
            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Subject)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Result relationships
            builder.Entity<Result>()
                .HasOne(r => r.Student)
                .WithMany(s => s.Results)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Result>()
                .HasOne(r => r.Exam)
                .WithMany(e => e.Results)
                .HasForeignKey(r => r.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Exam-Subject relationship
            builder.Entity<Exam>()
                .HasOne(e => e.Subject)
                .WithMany(s => s.Exams)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attendance relationships
            builder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraints
            builder.Entity<Student>()
                .HasIndex(s => s.StudentId)
                .IsUnique();

            builder.Entity<Teacher>()
                .HasIndex(t => t.TeacherId)
                .IsUnique();

            builder.Entity<Course>()
                .HasIndex(c => c.CourseCode)
                .IsUnique();

            builder.Entity<Subject>()
                .HasIndex(s => s.SubjectCode)
                .IsUnique();

            // Seed initial data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed Courses
            builder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    CourseCode = "BCA",
                    CourseName = "Bachelor of Computer Applications",
                    Description = "3-year undergraduate course in Computer Applications",
                    Duration = 6,
                    Department = "Computer Science",
                    IsActive = true
                },
                new Course
                {
                    Id = 2,
                    CourseCode = "MCA",
                    CourseName = "Master of Computer Applications",
                    Description = "2-year postgraduate course in Computer Applications",
                    Duration = 4,
                    Department = "Computer Science",
                    IsActive = true
                },
                new Course
                {
                    Id = 3,
                    CourseCode = "MBA",
                    CourseName = "Master of Business Administration",
                    Description = "2-year postgraduate course in Business Administration",
                    Duration = 4,
                    Department = "Management",
                    IsActive = true
                },
                new Course
                {
                    Id = 4,
                    CourseCode = "BBA",
                    CourseName = "Bachelor of Business Administration",
                    Description = "3-year undergraduate course in Business Administration",
                    Duration = 6,
                    Department = "Management",
                    IsActive = true
                }
            );

            // Seed Subjects for BCA Course
            builder.Entity<Subject>().HasData(
                // Semester 1
                new Subject { Id = 1, SubjectCode = "BCA101", SubjectName = "Programming in C", Credits = 4, CourseId = 1, Semester = 1, IsActive = true },
                new Subject { Id = 2, SubjectCode = "BCA102", SubjectName = "Computer Fundamentals", Credits = 3, CourseId = 1, Semester = 1, IsActive = true },
                new Subject { Id = 3, SubjectCode = "BCA103", SubjectName = "Mathematics-I", Credits = 4, CourseId = 1, Semester = 1, IsActive = true },
                new Subject { Id = 4, SubjectCode = "BCA104", SubjectName = "English Communication", Credits = 3, CourseId = 1, Semester = 1, IsActive = true },

                // Semester 2
                new Subject { Id = 5, SubjectCode = "BCA201", SubjectName = "Programming in C++", Credits = 4, CourseId = 1, Semester = 2, IsActive = true },
                new Subject { Id = 6, SubjectCode = "BCA202", SubjectName = "Data Structures", Credits = 4, CourseId = 1, Semester = 2, IsActive = true },
                new Subject { Id = 7, SubjectCode = "BCA203", SubjectName = "Mathematics-II", Credits = 4, CourseId = 1, Semester = 2, IsActive = true },
                new Subject { Id = 8, SubjectCode = "BCA204", SubjectName = "Database Management Systems", Credits = 4, CourseId = 1, Semester = 2, IsActive = true }
            );

            // Seed Teachers
            builder.Entity<Teacher>().HasData(
                new Teacher
                {
                    Id = 1,
                    TeacherId = "T001",
                    FirstName = "Dr. Rajesh",
                    LastName = "Kumar",
                    Email = "rajesh.kumar@srisaicollege.edu.in",
                    Phone = "9876543210",
                    Qualification = "Ph.D in Computer Science",
                    Department = "Computer Science",
                    JoiningDate = new DateTime(2020, 1, 1),
                    Experience = 10,
                    Salary = 75000,
                    IsActive = true
                },
                new Teacher
                {
                    Id = 2,
                    TeacherId = "T002",
                    FirstName = "Prof. Sita",
                    LastName = "Sharma",
                    Email = "sita.sharma@srisaicollege.edu.in",
                    Phone = "9876543211",
                    Qualification = "M.Tech in Information Technology",
                    Department = "Computer Science",
                    JoiningDate = new DateTime(2019, 6, 15),
                    Experience = 8,
                    Salary = 65000,
                    IsActive = true
                },
                new Teacher
                {
                    Id = 3,
                    TeacherId = "T003",
                    FirstName = "Dr. Amit",
                    LastName = "Verma",
                    Email = "amit.verma@srisaicollege.edu.in",
                    Phone = "9876543212",
                    Qualification = "MBA, Ph.D in Management",
                    Department = "Management",
                    JoiningDate = new DateTime(2018, 8, 10),
                    Experience = 12,
                    Salary = 80000,
                    IsActive = true
                }
            );

            // Seed Announcements
            builder.Entity<Announcement>().HasData(
                new Announcement
                {
                    Id = 1,
                    Title = "Welcome to New Academic Session 2024-25",
                    Content = "Sri Sai College of IT and Management welcomes all students to the new academic session. Classes will commence from July 15, 2024.",
                    CreatedDate = DateTime.Now,
                    Priority = "High",
                    Category = "Academic",
                    IsActive = true,
                    CreatedBy = "Admin"
                },
                new Announcement
                {
                    Id = 2,
                    Title = "Mid-Semester Examinations Schedule",
                    Content = "Mid-semester examinations will be conducted from October 15-25, 2024. Students are advised to prepare accordingly.",
                    CreatedDate = DateTime.Now,
                    Priority = "High",
                    Category = "Exam",
                    IsActive = true,
                    CreatedBy = "Admin"
                }
            );
        }
    }
}
