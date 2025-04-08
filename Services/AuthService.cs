using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using StudentInformationManagementSystem.Interfaces;
using StudentInformationManagementSystem.Models;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return null;

            bool isPasswordValid = await VerifyPasswordAsync(password, user.PasswordHash, user.Salt);

            if (!isPasswordValid)
                return null;

            // Update last login time
            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return user;
        }

        public async Task<(string hash, string salt)> HashPasswordAsync(string password)
        {
            // Generate a random salt
            byte[] saltBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            string salt = Convert.ToBase64String(saltBytes);

            // Hash the password with the salt
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return (hash, salt);
        }

        public async Task<bool> VerifyPasswordAsync(string password, string hash, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            string computedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return computedHash == hash;
        }

        public async Task<bool> IsAuthorizedAsync(User user, string roleName)
        {
            if (user == null || string.IsNullOrEmpty(roleName))
                return false;

            // Get the user with role details
            var userWithRole = await _userRepository.GetByIdAsync(user.UserId);

            return userWithRole?.Role?.Name == roleName;
        }
    }
}