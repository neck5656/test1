using Xunit;
using Microsoft.EntityFrameworkCore;
using StudentInformationManagementSystem.Data;
using StudentInformationManagementSystem.Models;
using StudentInformationManagementSystem.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace StudentInformationManagementSystem.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public UserRepositoryTests()
        {
            // Create unique database name for isolation between test methods
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestUserDb_{System.Guid.NewGuid()}")
                .Options;

            // Seed database with test data
            using (var context = new ApplicationDbContext(_options))
            {
                context.Roles.Add(new Role { RoleId = 1, Name = "Admin", Description = "Administrator" });
                context.Roles.Add(new Role { RoleId = 2, Name = "Faculty", Description = "Faculty Member" });
                context.Roles.Add(new Role { RoleId = 3, Name = "Student", Description = "Student" });

                context.Users.Add(new User
                {
                    UserId = 1,
                    Username = "testadmin",
                    Email = "admin@test.com",
                    PasswordHash = "hash",
                    Salt = "salt",
                    RoleId = 1,
                    IsActive = true
                });

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetByUsernameAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            // Act
            var user = await repository.GetByUsernameAsync("testadmin");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("testadmin", user.Username);
            Assert.Equal("admin@test.com", user.Email);
            Assert.NotNull(user.Role);
            Assert.Equal("Admin", user.Role.Name);
        }

        [Fact]
        public async Task CreateAsync_ValidUser_ReturnsUserId()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new UserRepository(context);

            var newUser = new User
            {
                Username = "newuser",
                Email = "new@test.com",
                PasswordHash = "hash",
                Salt = "salt",
                RoleId = 3,
                IsActive = true
            };

            // Act
            var userId = await repository.CreateAsync(newUser);

            // Assert
            Assert.True(userId > 0);

            // Verify user was added to database
            var savedUser = await context.Users.FindAsync(userId);
            Assert.NotNull(savedUser);
            Assert.Equal("newuser", savedUser.Username);
            Assert.Equal("new@test.com", savedUser.Email);
        }
    }
}