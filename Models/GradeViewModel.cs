// Models/GradeViewModel.cs
namespace StudentInformationManagementSystem.Models
{
    public class GradeViewModel
    {
        public int StudentCourseId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Grade { get; set; }
    }
}
