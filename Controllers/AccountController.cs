using Microsoft.AspNetCore.Mvc;
using StudentInformationManagementSystem.Interfaces;
using StudentInformationManagementSystem.Models;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserFactory _userFactory;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserFactory userFactory, ILogger<AccountController> logger)
        {
            _userFactory = userFactory;
            _logger = logger;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                // First check model validity
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state invalid during registration");
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning($"Error in {state.Key}: {error.ErrorMessage}");
                        }
                    }
                    return View(model);
                }

                // Check for required fields
                if (string.IsNullOrEmpty(model.Username) ||
                    string.IsNullOrEmpty(model.Email) ||
                    string.IsNullOrEmpty(model.Password) ||
                    string.IsNullOrEmpty(model.FirstName) ||
                    string.IsNullOrEmpty(model.LastName) ||
                    !model.DateOfBirth.HasValue)
                {
                    _logger.LogWarning("Required fields missing in registration");
                    ModelState.AddModelError("", "All required fields must be filled in.");
                    return View(model);
                }

                // Extra validation for phone number (digits only)
                if (!string.IsNullOrEmpty(model.PhoneNumber) && !Regex.IsMatch(model.PhoneNumber, @"^[0-9]*$"))
                {
                    _logger.LogWarning("Phone number contains non-digit characters");
                    ModelState.AddModelError("PhoneNumber", "Phone number must contain only digits.");
                    return View(model);
                }

                // Create a new student user with all the information
                _logger.LogInformation($"Creating new student user with username: {model.Username}");
                var user = await _userFactory.CreateStudentUserAsync(
                    model.Username,
                    model.Email,
                    model.Password,
                    model.FirstName,
                    model.LastName,
                    model.DateOfBirth,
                    model.Address ?? "",
                    model.PhoneNumber
                );

                // Redirect to login page with success message
                _logger.LogInformation($"User {model.Username} created successfully with ID: {user.UserId}");
                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Login", "Auth");
            }
            catch (InvalidOperationException ex)
            {
                // This catches username/email already exists exceptions
                _logger.LogWarning(ex, "Registration failed due to duplicate username/email");
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                // Log the actual exception for debugging
                _logger.LogError(ex, "Error during registration");

                // Always show the specific error in this case since we're troubleshooting
                ModelState.AddModelError("", $"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    ModelState.AddModelError("", $"Inner Error: {ex.InnerException.Message}");
                }

                return View(model);
            }
        }
    }
}