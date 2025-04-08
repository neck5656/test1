using Microsoft.AspNetCore.Mvc;
using StudentInformationManagementSystem.Interfaces;
using System;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Controllers
{
    // This controller is for creating an initial admin account
    // It should be removed or secured after initial setup!
    public class SetupController : Controller
    {
        private readonly IUserFactory _userFactory;
        private readonly IUserRepository _userRepository;

        public SetupController(IUserFactory userFactory, IUserRepository userRepository)
        {
            _userFactory = userFactory;
            _userRepository = userRepository;
        }

        // This action creates an admin account with credentials:
        // Username: admin
        // Email: admin@example.com
        // Password: Admin123!
        public async Task<IActionResult> CreateAdminAccount()
        {
            try
            {
                // Check if admin already exists
                var existingAdmin = await _userRepository.GetByUsernameAsync("admin");
                if (existingAdmin != null)
                {
                    return Content("Admin account already exists!");
                }

                // Create the admin account
                var adminUser = await _userFactory.CreateAdminUserAsync(
                    "admin",
                    "admin@example.com",
                    "Admin123!"
                );

                return Content("Admin account created successfully! Username: admin, Password: Admin123!");
            }
            catch (Exception ex)
            {
                return Content($"Error creating admin account: {ex.Message}");
            }
        }
    }
}