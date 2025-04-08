using Xunit;
using Microsoft.EntityFrameworkCore;
using StudentInformationManagementSystem.Data;
using StudentInformationManagementSystem.Models;
using StudentInformationManagementSystem.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Tests.Services
{
    public class CourseManagerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public CourseManagerTests()
        {
            // Create unique database name for test isolation
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestCourseManager_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddCourseAsync_ValidCourse_ReturnsCourseId()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_options))
            {
                var courseManager = CourseManager.GetInstance(context);
                var newCourse = new Course
                {
                    CourseCode = "CS301",
                    CourseName = "Software Engineering",
                    Description = "Software development methodologies", // Make sure Description is set
                    CreditHours = 3,
                    IsActive = true
                };

                // Act
                var courseId = await courseManager.AddCourseAsync(newCourse);

                // Assert
                Assert.True(courseId > 0);

                // Verify course was added to database
                var savedCourse = await context.Courses.FindAsync(courseId);
                Assert.NotNull(savedCourse);
                Assert.Equal("CS301", savedCourse.CourseCode);
                Assert.Equal("Software Engineering", savedCourse.CourseName);
                Assert.Equal("Software development methodologies", savedCourse.Description);
                Assert.NotEqual(default(DateTime), savedCourse.CreatedDate);
            }
        }

        [Fact]
        public async Task GetAllCoursesAsync_ReturnsCoursesList()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_options))
            {
                context.Courses.Add(new Course
                {
                    CourseId = 1,
                    CourseCode = "CS101",
                    CourseName = "Introduction to Programming",
                    Description = "Basic programming concepts", // Make sure Description is set
                    CreditHours = 3,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-1)
                });

                context.Courses.Add(new Course
                {
                    CourseId = 2,
                    CourseCode = "CS201",
                    CourseName = "Data Structures",
                    Description = "Advanced data structures", // Make sure Description is set
                    CreditHours = 4,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                });

                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(_options))
            {
                var courseManager = CourseManager.GetInstance(context);
                var courses = await courseManager.GetAllCoursesAsync();

                // Assert
                Assert.Equal(2, courses.Count);
                Assert.Contains(courses, c => c.CourseCode == "CS101");
                Assert.Contains(courses, c => c.CourseCode == "CS201");
            }
        }
    }
}