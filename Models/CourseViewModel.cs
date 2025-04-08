using System;

namespace StudentInformationManagementSystem.Models
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public int CreditHours { get; set; }
        public string Grade { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
