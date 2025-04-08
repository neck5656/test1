using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace StudentInformationManagementSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRolesAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRolesAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata
                .Any(em => em.GetType().Name == "AllowAnonymousAttribute");

            if (allowAnonymous)
                return;

            // Check if the user is authenticated
            var userId = context.HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Not logged in - redirect to login
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // Get the user role from session
            var userRole = context.HttpContext.Session.GetString("UserRole");

            // Check if the user has any of the required roles
            if (!_roles.Contains(userRole))
            {
                // User doesn't have the required role - redirect to access denied
                context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
            }
        }
    }
}