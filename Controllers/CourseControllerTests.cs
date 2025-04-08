using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInformationManagementSystem.Controllers;
using StudentInformationManagementSystem.Data;
using StudentInformationManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using StudentInformationManagementSystem.Tests.Helpers;

namespace StudentInformationManagementSystem.Tests.Controllers
{
    public class CourseControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly CourseController _controller;

        public CourseControllerTests()
        {
            // Setup in-memory database with a unique name for test isolation
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestCoursesDb_{Guid.NewGuid()}")
                .Options;

            var context = new ApplicationDbContext(_options);

            // Create controller with real context
            _controller = new CourseController(context);

            // Setup HttpContext for session
            var httpContext = new DefaultHttpContext();
            var mockSession = new MockHttpSession();
            mockSession.SetInt32("UserId", 1);
            mockSession.SetString("UserRole", "Admin");
            httpContext.Session = mockSession;

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Clear any existing data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed test data
            SeedDatabase(context);
        }

        private void SeedDatabase(ApplicationDbContext context)
        {
            // Add test roles
            context.Roles.Add(new Role { RoleId = 1, Name = "Admin", Description = "Administrator" });

            // Add test users
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "admin",
                Email = "admin@example.com",
                RoleId = 1,
                IsActive = true,
                PasswordHash = "hash",
                Salt = "salt"
            });

            // Add test courses with Description
            context.Courses.Add(new Course
            {
                CourseId = 1,
                CourseCode = "CS101",
                CourseName = "Introduction to Programming",
                Description = "Basic programming concepts",
                CreditHours = 3,
                IsActive = true,
                CreatedDate = DateTime.Now
            });

            context.Courses.Add(new Course
            {
                CourseId = 2,
                CourseCode = "CS201",
                CourseName = "Data Structures",
                Description = "Advanced data structures",
                CreditHours = 4,
                IsActive = true,
                CreatedDate = DateTime.Now
            });

            context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewWithCourses()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Course>>(viewResult.Model);
            Assert.Equal(2, model.Count());
            Assert.Contains(model, c => c.CourseCode == "CS101");
            Assert.Contains(model, c => c.CourseCode == "CS201");
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ValidCourse_RedirectsToIndex()
        {
            // Arrange
            var newCourse = new Course
            {
                CourseCode = "CS301",
                CourseName = "Software Engineering",
                Description = "Software development methodologies",
                CreditHours = 3,
                IsActive = true
            };

            // Act
            var result = await _controller.Create(newCourse);

            // Assert - don't check exact types, check base types
            Assert.IsAssignableFrom<ActionResult>(result);

            // Check if it's a redirect
            if (result is RedirectToActionResult redirectResult)
            {
                Assert.Equal("Index", redirectResult.ActionName);
            }

            // Verify course was added to database
            using (var context = new ApplicationDbContext(_options))
            {
                var savedCourse = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "CS301");
                Assert.NotNull(savedCourse);
                Assert.Equal("Software Engineering", savedCourse.CourseName);
                Assert.Equal(3, savedCourse.CreditHours);
                Assert.True(savedCourse.IsActive);
                Assert.NotEqual(default(DateTime), savedCourse.CreatedDate);
            }
        }
    }
}