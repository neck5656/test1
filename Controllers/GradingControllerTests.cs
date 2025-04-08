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
    public class GradingControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly GradingController _controller;

        public GradingControllerTests()
        {
            // Setup in-memory database with a unique name for test isolation
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestGradingDb_{Guid.NewGuid()}")
                .Options;

            // Create the database and seed it with test data
            using (var dbContext = new ApplicationDbContext(_options))
            {
                // Add test courses
                var course = new Course
                {
                    CourseId = 1,
                    CourseCode = "CS101",
                    CourseName = "Introduction to Programming",
                    Description = "Basic programming concepts", // Make sure Description is set
                    CreditHours = 3,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-30)
                };
                dbContext.Courses.Add(course);

                // Add test roles
                var studentRole = new Role { RoleId = 3, Name = "Student", Description = "Student Role" };
                dbContext.Roles.Add(studentRole);

                // Add test users
                var user = new User
                {
                    UserId = 1,
                    Username = "testuser",
                    Email = "test@example.com",
                    RoleId = 3,
                    IsActive = true,
                    PasswordHash = "hash",
                    Salt = "salt"
                };
                dbContext.Users.Add(user);

                // Add test students
                var student = new Student
                {
                    StudentId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    StudentNumber = "STU-2025-12345",
                    UserId = 1,
                    EnrollmentDate = DateTime.Now.AddYears(-1),
                    PhoneNumber = "1234567890",
                    Address = "123 Test St"
                };
                dbContext.Students.Add(student);

                // Add test enrollments - initialize Grade property to empty string instead of null
                var enrollment = new StudentCourse
                {
                    StudentCourseId = 1,
                    StudentId = 1,
                    CourseId = 1,
                    EnrollmentDate = DateTime.Now,
                    IsActive = true,
                    Grade = "" // Initialize with empty string instead of null
                };
                dbContext.StudentCourses.Add(enrollment);

                dbContext.SaveChanges();
            }

            // Create a new context for the controller
            var context = new ApplicationDbContext(_options);
            _controller = new GradingController(context);

            // Setup session
            var httpContext = new DefaultHttpContext();
            var session = new MockHttpSession();
            session.SetInt32("UserId", 2); // Faculty user ID
            session.SetString("UserRole", "Faculty");
            httpContext.Session = session;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task Index_ReturnsViewWithCourses()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Course>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal("CS101", model.First().CourseCode);
        }

        [Fact]
        public async Task CourseStudents_ValidCourseId_ReturnsViewWithStudents()
        {
            // Act
            var result = await _controller.CourseStudents(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<StudentCourse>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal(1, model.First().StudentId);
            Assert.Equal("Introduction to Programming", viewResult.ViewData["CourseName"]?.ToString());
        }

        [Fact]
        public async Task SubmitGrade_Get_ReturnsViewWithEnrollment()
        {
            // Act
            var result = await _controller.SubmitGrade(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StudentCourse>(viewResult.Model);
            Assert.Equal(1, model.StudentCourseId);
            Assert.Equal("", model.Grade); // Check for empty string, not null
        }

        [Fact]
        public async Task SubmitGrade_Post_UpdatesGradeAndRedirects()
        {
            // Act
            var result = await _controller.SubmitGrade(1, "A");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("CourseStudents", redirectResult.ActionName);

            // Verify grade was updated in the database
            using (var dbContext = new ApplicationDbContext(_options))
            {
                var enrollment = await dbContext.StudentCourses.FindAsync(1);
                Assert.NotNull(enrollment);
                Assert.Equal("A", enrollment.Grade);
            }
        }
    }
}