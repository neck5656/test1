using StudentInformationManagementSystem.Interfaces;
using StudentInformationManagementSystem.Models;

namespace StudentInformationManagementSystem.Services
{
    public class GradeNotificationObserver : IGradeObserver
    {
        private readonly ILogger<GradeNotificationObserver> _logger;

        public GradeNotificationObserver(ILogger<GradeNotificationObserver> logger)
        {
            _logger = logger;
        }

        public void Update(StudentCourse studentCourse)
        {
            // In a real application, this would send an email or notification
            _logger.LogInformation($"Notification: Grade updated for student {studentCourse.StudentId} in course {studentCourse.CourseId} to {studentCourse.Grade}");
        }
    }
}