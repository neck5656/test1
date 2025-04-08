using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentInformationManagementSystem.Models
{
    public class Course
    {
        public Course()
        {
            // Removed: StudentCourses initialization
        }

        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, 6)]
        [Display(Name = "Credit Hours")]
        public int CreditHours { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Removed: Navigation property for student enrollments
    }
}