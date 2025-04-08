using Microsoft.AspNetCore.Http;
using StudentInformationManagementSystem.Interfaces;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
        {
            // Check if user is logged in (has a user ID in session)
            if (context.Session.GetInt32("UserId") != null)
            {
                var userId = context.Session.GetInt32("UserId").Value;

                // Get the user from the repository
                var user = await userRepository.GetByIdAsync(userId);

                // If the user exists, set the context items for easy access in controllers
                if (user != null)
                {
                    context.Items["CurrentUser"] = user;
                    context.Items["UserRole"] = user.Role.Name;
                }
            }

            await _next(context);
        }
    }
}