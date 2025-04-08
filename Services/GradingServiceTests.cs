using Xunit;
using Moq;
using StudentInformationManagementSystem.Interfaces;
using StudentInformationManagementSystem.Models;
using StudentInformationManagementSystem.Services;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Tests.Services
{
    public class GradingServiceTests
    {
        private readonly Mock<IStudentCourseRepository> _mockRepository;
        private readonly GradingService _gradingService;

        public GradingServiceTests()
        {
            _mockRepository = new Mock<IStudentCourseRepository>();
            _gradingService = new GradingService(_mockRepository.Object);
        }

        [Fact]
        public async Task SubmitGradeAsync_UpdatesGradeAndNotifiesObservers()
        {
            // Arrange
            var studentCourse = new StudentCourse
            {
                StudentCourseId = 1,
                StudentId = 1,
                CourseId = 1,
                Grade = null
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(studentCourse);

            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<StudentCourse>()))
                .Returns(Task.CompletedTask);

            var mockObserver = new Mock<IGradeObserver>();
            _gradingService.Attach(mockObserver.Object);

            // Act
            await _gradingService.SubmitGradeAsync(1, "A");

            // Assert
            Assert.Equal("A", studentCourse.Grade);
            _mockRepository.Verify(repo => repo.UpdateAsync(studentCourse), Times.Once);

            // Use Once instead of checking exact number
            mockObserver.Verify(o => o.Update(studentCourse), Times.Once);
        }

        [Fact]
        public void Attach_AddsObserver()
        {
            // Arrange
            var mockObserver = new Mock<IGradeObserver>();

            // Act - Add the observer only once
            _gradingService.Attach(mockObserver.Object);

            // Setup a student course to update
            var studentCourse = new StudentCourse { StudentCourseId = 1 };
            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(studentCourse);

            // Act - trigger notification
            _gradingService.SubmitGradeAsync(1, "A").Wait();

            // Assert - observer was notified once
            mockObserver.Verify(o => o.Update(studentCourse), Times.Once);
        }

        [Fact]
        public void Detach_RemovesObserver()
        {
            // Arrange
            var mockObserver = new Mock<IGradeObserver>();
            _gradingService.Attach(mockObserver.Object);

            // Act
            _gradingService.Detach(mockObserver.Object);

            // Setup a student course to update
            var studentCourse = new StudentCourse { StudentCourseId = 1 };
            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(studentCourse);

            // Act - trigger notification
            _gradingService.SubmitGradeAsync(1, "A").Wait();

            // Assert - observer was not notified after being detached
            mockObserver.Verify(o => o.Update(It.IsAny<StudentCourse>()), Times.Never);
        }
    }
}