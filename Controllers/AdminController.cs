using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInformationManagementSystem.Attributes;
using StudentInformationManagementSystem.Data;
using StudentInformationManagementSystem.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace StudentInformationManagementSystem.Controllers
{
    [AuthorizeRoles("Admin")]
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public AdminController(IUserRepository userRepository, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Get current admin user
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var user = await _userRepository.GetByIdAsync(userId);
                ViewBag.UserName = user?.Username ?? "Admin";

                // Count total users
                var allUsers = await _userRepository.GetAllAsync();
                ViewBag.TotalUsers = allUsers.Count();

                // Count students (users with RoleId = 3)
                ViewBag.TotalStudents = allUsers.Count(u => u.RoleId == 3);

                // Count faculty (users with RoleId = 2)
                ViewBag.TotalFaculty = allUsers.Count(u => u.RoleId == 2);

                // Get courses count separately
                var coursesCount = await _context.Courses.CountAsync();
                ViewBag.TotalCourses = coursesCount;

                return View();
            }
            catch (System.Exception ex)
            {
                // Log the error
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // GET: Admin/ManageUsers
        public async Task<IActionResult> ManageUsers()
        {
            try
            {
                // Get current admin user for the ViewBag
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (userId != 0)
                {
                    var currentUser = await _userRepository.GetByIdAsync(userId);
                    ViewBag.UserName = currentUser?.Username ?? "Admin";
                }

                var users = await _userRepository.GetAllAsync();
                return View(users);
            }
            catch (System.Exception ex)
            {
                // Log the error
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
    }
}