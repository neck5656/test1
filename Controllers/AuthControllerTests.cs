using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using StudentInformationManagementSystem.Controllers;
using StudentInformationManagementSystem.Interfaces;
using StudentInformationManagementSystem.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudentInformationManagementSystem.Tests.Helpers;

namespace StudentInformationManagementSystem.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);

            // Setup controller context with session
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockHttpSession();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public void Login_Get_ReturnsView()
        {
            // Act
            var result = _controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Default view
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_RedirectsBasedOnRole()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "testuser",
                Password = "password",
                RememberMe = false
            };

            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                Role = new Role { RoleId = 3, Name = "Student" }
            };

            _mockAuthService.Setup(auth => auth.AuthenticateAsync("testuser", "password"))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Login(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("StudentCourse", redirectResult.ControllerName);

            // Verify session was set
            var session = _controller.HttpContext.Session;
            Assert.Equal(1, session.GetInt32("UserId"));
            Assert.Equal("testuser", session.GetString("Username"));
            Assert.Equal("Student", session.GetString("UserRole"));
        }

        [Fact]
        public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Username = "wronguser",
                Password = "wrongpassword",
                RememberMe = false
            };

            _mockAuthService.Setup(auth => auth.AuthenticateAsync("wronguser", "wrongpassword"))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<LoginViewModel>(viewResult.Model);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey(""));
        }

        [Fact]
        public void Logout_ClearsSessionAndRedirectsToLogin()
        {
            // Arrange
            var session = (MockHttpSession)_controller.HttpContext.Session;
            session.SetInt32("UserId", 1);
            session.SetString("Username", "testuser");
            session.SetString("UserRole", "Student");

            // Act
            var result = _controller.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("Auth", redirectResult.ControllerName);

            // Verify session was cleared
            Assert.Empty(session.Keys);
        }
    }
}