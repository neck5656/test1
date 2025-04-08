using StudentInformationManagementSystem.Models;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Interfaces
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<(string hash, string salt)> HashPasswordAsync(string password);
        Task<bool> VerifyPasswordAsync(string password, string hash, string salt);
        Task<bool> IsAuthorizedAsync(User user, string roleName);
    }
}