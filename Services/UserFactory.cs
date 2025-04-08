using StudentInformationManagementSystem.Interfaces;
using StudentInformationManagementSystem.Models;
using System;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Services
{
    public class UserFactory : IUserFactory
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly IStudentRepository _studentRepository;
        private static readonly Random _random = new Random();

        public UserFactory(IUserRepository userRepository, IAuthService authService, IStudentRepository studentRepository)
        {
            _userRepository = userRepository;
            _authService = authService;
            _studentRepository = studentRepository;
        }

        public async Task<User> CreateUserAsync(string username, string email, string password, string roleName)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(roleName))
                throw new ArgumentException("All fields are required");

            // Check if username or email already exists
            if (await _userRepository.UsernameExistsAsync(username))
                throw new InvalidOperationException("Username already exists");

            if (await _userRepository.EmailExistsAsync(email))
                throw new InvalidOperationException("Email already exists");

            // Hash the password
            var (hash, salt) = await _authService.HashPasswordAsync(password);

            // Create the user object
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hash,
                Salt = salt,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Get role ID based on role name and assign it
            switch (roleName.ToLower())
            {
                case "admin":
                    user.RoleId = 1;
                    break;
                case "faculty":
                    user.RoleId = 2;
                    break;
                case "student":
                    user.RoleId = 3;
                    break;
                default:
                    throw new ArgumentException("Invalid role name");
            }

            // Save the user to the database
            user.UserId = await _userRepository.CreateAsync(user);

            return user;
        }

        public async Task<User> CreateStudentUserAsync(
            string username,
            string email,
            string password,
            string firstName,
            string lastName,
            DateTime? dateOfBirth, // Made nullable but will be validated
            string address = "",
            string phoneNumber = "",
            string studentNumber = "")
        {
            // Create base user with student role
            var user = await CreateUserAsync(username, email, password, "student");

            // Validate Date of Birth - it should not be null by the time it gets here due to validation
            if (!dateOfBirth.HasValue)
            {
                throw new ArgumentException("Date of Birth is required");
            }

            // Generate a random student number if not provided
            if (string.IsNullOrEmpty(studentNumber))
            {
                // Format: STU-{Current Year}-{Random 5-digit number}
                studentNumber = $"STU-{DateTime.Now.Year}-{_random.Next(10000, 99999)}";
            }

            // Create student profile
            var student = new Student
            {
                UserId = user.UserId,
                FirstName = firstName,
                LastName = lastName,
                Address = address ?? "",
                PhoneNumber = phoneNumber,
                DateOfBirth = dateOfBirth, // Will not be null due to validation
                StudentNumber = studentNumber,
                EnrollmentDate = DateTime.UtcNow
            };

            // Save the student profile to the database
            await _studentRepository.CreateAsync(student);

            return user;
        }

        public async Task<User> CreateFacultyUserAsync(string username, string email, string password)
        {
            return await CreateUserAsync(username, email, password, "faculty");
        }

        public async Task<User> CreateAdminUserAsync(string username, string email, string password)
        {
            return await CreateUserAsync(username, email, password, "admin");
        }
    }
}