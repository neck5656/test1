using Xunit;
using Microsoft.EntityFrameworkCore;
using StudentInformationManagementSystem.Data;
using StudentInformationManagementSystem.Models;
using StudentInformationManagementSystem.Repositories;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace StudentInformationManagementSystem.Tests.Repositories
{
    public class StudentRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public StudentRepositoryTests()
        {
            // Create unique database name for test isolation
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestStudentDb_{Guid.NewGuid()}")
                .Options;

            // Seed the database with test data
            using (var context = new ApplicationDbContext(_options))
            {
                // Add test role
                var role = new Role
                {
                    RoleId = 3,
                    Name = "Student",
                    Description = "Student role"
                };
                context.Roles.Add(role);

                // Add test users
                var user1 = new User
                {
                    UserId = 1,
                    Username = "student1",
                    Email = "student1@example.com",
                    PasswordHash = "hash",
                    Salt = "salt",
                    RoleId = 3,
                    IsActive = true
                };

                var user2 = new User
                {
                    UserId = 2,
                    Username = "student2",
                    Email = "student2@example.com",
                    PasswordHash = "hash",
                    Salt = "salt",
                    RoleId = 3,
                    IsActive = true
                };

                context.Users.Add(user1);
                context.Users.Add(user2);

                // Add test students
                var student1 = new Student
                {
                    StudentId = 1,
                    UserId = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    Address = "123 Main St",
                    PhoneNumber = "1234567890",
                    StudentNumber = "STU-2025-12345",
                    EnrollmentDate = DateTime.Now.AddMonths(-6)
                };

                var student2 = new Student
                {
                    StudentId = 2,
                    UserId = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    DateOfBirth = new DateTime(2001, 5, 10),
                    Address = "456 Oak St",
                    PhoneNumber = "9876543210",
                    StudentNumber = "STU-2025-67890",
                    EnrollmentDate = DateTime.Now.AddMonths(-3)
                };

                context.Students.Add(student1);
                context.Students.Add(student2);

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetByIdAsync_ExistingStudent_ReturnsStudent()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new StudentRepository(context);

            // Act
            var student = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(student);
            Assert.Equal("John", student.FirstName);
            Assert.Equal("Doe", student.LastName);
            Assert.Equal("STU-2025-12345", student.StudentNumber);
            Assert.NotNull(student.User);
            Assert.Equal("student1", student.User.Username);
            Assert.NotNull(student.User.Role);
            Assert.Equal("Student", student.User.Role.Name);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentStudent_ReturnsNull()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new StudentRepository(context);

            // Act
            var student = await repository.GetByIdAsync(999);

            // Assert
            Assert.Null(student);
        }

        [Fact]
        public async Task GetByUserIdAsync_ExistingUser_ReturnsStudent()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new StudentRepository(context);

            // Act
            var student = await repository.GetByUserIdAsync(1);

            // Assert
            Assert.NotNull(student);
            Assert.Equal("John", student.FirstName);
            Assert.Equal("Doe", student.LastName);
            Assert.Equal(1, student.UserId);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllStudents()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new StudentRepository(context);

            // Act
            var students = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, students.Count());
            Assert.Contains(students, s => s.FirstName == "John" && s.LastName == "Doe");
            Assert.Contains(students, s => s.FirstName == "Jane" && s.LastName == "Smith");
        }

        [Fact]
        public async Task CreateAsync_ValidStudent_ReturnsStudentId()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new StudentRepository(context);

            // Add a new user first
            var user = new User
            {
                UserId = 3,
                Username = "student3",
                Email = "student3@example.com",
                PasswordHash = "hash",
                Salt = "salt",
                RoleId = 3,
                IsActive = true
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var newStudent = new Student
            {
                UserId = 3,
                FirstName = "Robert",
                LastName = "Johnson",
                DateOfBirth = new DateTime(1999, 8, 15),
                Address = "789 Pine St",
                PhoneNumber = "5556667777",
                StudentNumber = "STU-2025-54321",
                EnrollmentDate = DateTime.Now
            };

            // Act
            var studentId = await repository.CreateAsync(newStudent);

            // Assert
            Assert.True(studentId > 0);

            // Verify student was added to database
            var savedStudent = await context.Students.FindAsync(studentId);
            Assert.NotNull(savedStudent);
            Assert.Equal("Robert", savedStudent.FirstName);
            Assert.Equal("Johnson", savedStudent.LastName);
            Assert.Equal("STU-2025-54321", savedStudent.StudentNumber);
        }

        [Fact]
        public async Task UpdateAsync_ExistingStudent_UpdatesStudent()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new StudentRepository(context);

            var student = await context.Students.FindAsync(1);
            student.FirstName = "Jonathan";
            student.LastName = "Dorian";
            student.Address = "Updated Address";
            student.PhoneNumber = "9998887777";

            // Act
            await repository.UpdateAsync(student);

            // Assert - Get fresh instance from database
            context.Entry(student).State = EntityState.Detached;
            var updatedStudent = await context.Students.FindAsync(1);

            Assert.Equal("Jonathan", updatedStudent.FirstName);
            Assert.Equal("Dorian", updatedStudent.LastName);
            Assert.Equal("Updated Address", updatedStudent.Address);
            Assert.Equal("9998887777", updatedStudent.PhoneNumber);
        }

        [Fact]
        public async Task DeleteAsync_ExistingStudent_RemovesStudent()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new StudentRepository(context);

            // Act
            await repository.DeleteAsync(1);

            // Assert
            var deletedStudent = await context.Students.FindAsync(1);
            Assert.Null(deletedStudent);
        }
    }
}