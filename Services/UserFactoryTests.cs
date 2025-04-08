using Xunit;
using Moq;
using StudentInformationManagementSystem.Interfaces;
using StudentInformationManagementSystem.Models;
using StudentInformationManagementSystem.Services;
using System;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Tests.Services
{
    public class UserFactoryTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly UserFactory _userFactory;

        public UserFactoryTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockAuthService = new Mock<IAuthService>();
            _mockStudentRepository = new Mock<IStudentRepository>();

            _userFactory = new UserFactory(
                _mockUserRepository.Object,
                _mockAuthService.Object,
                _mockStudentRepository.Object);
        }

        [Fact]
        public async Task CreateUserAsync_ValidData_ReturnsUser()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.UsernameExistsAsync("testuser"))
                .ReturnsAsync(false);

            _mockUserRepository.Setup(repo => repo.EmailExistsAsync("test@example.com"))
                .ReturnsAsync(false);

            _mockAuthService.Setup(auth => auth.HashPasswordAsync("Password123!"))
                .ReturnsAsync(("hashedPassword", "salt"));

            _mockUserRepository.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(1);

            // Act
            var result = await _userFactory.CreateUserAsync(
                "testuser",
                "test@example.com",
                "Password123!",
                "Student");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal(3, result.RoleId); // Student role ID

            _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateStudentUserAsync_ValidData_ReturnsUser()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.UsernameExistsAsync("testuser"))
                .ReturnsAsync(false);

            _mockUserRepository.Setup(repo => repo.EmailExistsAsync("test@example.com"))
                .ReturnsAsync(false);

            _mockAuthService.Setup(auth => auth.HashPasswordAsync("Password123!"))
                .ReturnsAsync(("hashedPassword", "salt"));

            _mockUserRepository.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(1);

            _mockStudentRepository.Setup(repo => repo.CreateAsync(It.IsAny<Student>()))
                .ReturnsAsync(1);

            // Act
            var result = await _userFactory.CreateStudentUserAsync(
                "testuser",
                "test@example.com",
                "Password123!",
                "Test",
                "User",
                new DateTime(2000, 1, 1),
                "123 Main St",
                "1234567890");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal(3, result.RoleId); // Student role ID

            _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
            _mockStudentRepository.Verify(repo => repo.CreateAsync(It.Is<Student>(s =>
                s.UserId == 1 &&
                s.FirstName == "Test" &&
                s.LastName == "User" &&
                s.DateOfBirth == new DateTime(2000, 1, 1) &&
                s.Address == "123 Main St" &&
                s.PhoneNumber == "1234567890")),
                Times.Once);
        }
    }
}