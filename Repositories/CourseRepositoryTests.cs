using Xunit;
using Microsoft.EntityFrameworkCore;
using StudentInformationManagementSystem.Data;
using StudentInformationManagementSystem.Models;
using StudentInformationManagementSystem.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using StudentInformationManagementSystem.Interfaces;

namespace StudentInformationManagementSystem.Tests.Repositories
{
    public class CourseRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public CourseRepositoryTests()
        {
            // Create unique database name for test isolation
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestCourseDb_{Guid.NewGuid()}")
                .Options;

            // Seed the database with test data
            using (var context = new ApplicationDbContext(_options))
            {
                context.Database.EnsureDeleted(); // Clear any existing data

                context.Courses.Add(new Course
                {
                    CourseId = 101, // Use different IDs to avoid conflicts
                    CourseCode = "CS101",
                    CourseName = "Introduction to Programming",
                    Description = "Basic programming concepts",
                    CreditHours = 3,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-30)
                });

                context.Courses.Add(new Course
                {
                    CourseId = 102,
                    CourseCode = "CS201",
                    CourseName = "Data Structures",
                    Description = "Advanced data structures",
                    CreditHours = 4,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-15)
                });

                context.Courses.Add(new Course
                {
                    CourseId = 103,
                    CourseCode = "CS301",
                    CourseName = "Database Systems",
                    Description = "Database design and SQL",
                    CreditHours = 3,
                    IsActive = false, // Inactive course
                    CreatedDate = DateTime.Now.AddDays(-45)
                });

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetByIdAsync_ExistingCourse_ReturnsCourse()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new CourseRepository(context);

            // Act
            var course = await repository.GetByIdAsync(101);

            // Assert
            Assert.NotNull(course);
            Assert.Equal("CS101", course.CourseCode);
            Assert.Equal("Introduction to Programming", course.CourseName);
            Assert.Equal(3, course.CreditHours);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentCourse_ReturnsNull()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new CourseRepository(context);

            // Act
            var course = await repository.GetByIdAsync(999);

            // Assert
            Assert.Null(course);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCourses()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new CourseRepository(context);

            // Act
            var courses = await repository.GetAllAsync();

            // Assert
            Assert.Equal(3, courses.Count());
            Assert.Contains(courses, c => c.CourseCode == "CS101");
            Assert.Contains(courses, c => c.CourseCode == "CS201");
            Assert.Contains(courses, c => c.CourseCode == "CS301");
        }

        [Fact]
        public async Task CreateAsync_ValidCourse_ReturnsCourseId()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new CourseRepository(context);

            var newCourse = new Course
            {
                // Don't specify CourseId - let the database generate it
                CourseCode = "CS401",
                CourseName = "Software Engineering",
                Description = "Software development methodologies",
                CreditHours = 3,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            // Act
            var courseId = await repository.CreateAsync(newCourse);

            // Assert
            Assert.True(courseId > 0);

            // Verify course was added to database
            var savedCourse = await context.Courses.FindAsync(courseId);
            Assert.NotNull(savedCourse);
            Assert.Equal("CS401", savedCourse.CourseCode);
            Assert.Equal("Software Engineering", savedCourse.CourseName);
        }

        [Fact]
        public async Task UpdateAsync_ExistingCourse_UpdatesCourse()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new CourseRepository(context);

            var course = await context.Courses.FindAsync(101);
            course.CourseName = "Updated Programming Course";
            course.Description = "Updated description";
            course.CreditHours = 4;

            // Act
            await repository.UpdateAsync(course);

            // Assert - Get fresh instance from database
            context.Entry(course).State = EntityState.Detached;
            var updatedCourse = await context.Courses.FindAsync(101);

            Assert.Equal("Updated Programming Course", updatedCourse.CourseName);
            Assert.Equal("Updated description", updatedCourse.Description);
            Assert.Equal(4, updatedCourse.CreditHours);
        }

        [Fact]
        public async Task DeleteAsync_ExistingCourse_SetsCourseInactive()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new CourseRepository(context);

            // Act
            await repository.DeleteAsync(101);

            // Assert
            var course = await context.Courses.FindAsync(101);
            Assert.NotNull(course); // Course still exists
            Assert.False(course.IsActive); // But is inactive
        }
    }
}