using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInformationManagementSystem.Attributes;
using StudentInformationManagementSystem.Data;
using StudentInformationManagementSystem.Models;
using StudentInformationManagementSystem.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StudentInformationManagementSystem.Controllers
{
    [AuthorizeRoles("Admin")]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CourseManager _courseManager;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
            _courseManager = CourseManager.GetInstance(context);
        }

        // GET: Course/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get current admin user for the ViewBag
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (userId != 0)
                {
                    var currentUser = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == userId);
                    ViewBag.UserName = currentUser?.Username ?? "Admin";
                }

                var courses = await _courseManager.GetAllCoursesAsync();
                return View(courses);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // GET: Course/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                // Get current admin user for the ViewBag
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (userId != 0)
                {
                    var currentUser = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == userId);
                    ViewBag.UserName = currentUser?.Username ?? "Admin";
                }

                var course = await _courseManager.GetCourseByIdAsync(id.Value);

                if (course == null)
                {
                    return NotFound();
                }

                return View(course);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // GET: Course/Create
        public IActionResult Create()
        {
            // Get current admin user for the ViewBag
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId != 0)
            {
                var currentUser = _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.UserId == userId);
                ViewBag.UserName = currentUser?.Username ?? "Admin";
            }

            return View();
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseCode,CourseName,Description,CreditHours,IsActive")] Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if course code already exists
                    bool codeExists = await _courseManager.CourseCodeExistsAsync(course.CourseCode);
                    if (codeExists)
                    {
                        ModelState.AddModelError("CourseCode", "This Course Code already exists.");
                        return View(course);
                    }

                    await _courseManager.AddCourseAsync(course);
                    return RedirectToAction(nameof(Index));
                }

                return View(course);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                // Get current admin user for the ViewBag
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (userId != 0)
                {
                    var currentUser = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == userId);
                    ViewBag.UserName = currentUser?.Username ?? "Admin";
                }

                var course = await _courseManager.GetCourseByIdAsync(id.Value);

                if (course == null)
                {
                    return NotFound();
                }

                return View(course);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,CourseCode,CourseName,Description,CreditHours,CreatedDate,IsActive")] Course course)
        {
            try
            {
                if (id != course.CourseId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    // Check if course code already exists (excluding this course)
                    bool codeExists = await _courseManager.CourseCodeExistsAsync(course.CourseCode, course.CourseId);

                    if (codeExists)
                    {
                        ModelState.AddModelError("CourseCode", "This Course Code already exists.");
                        return View(course);
                    }

                    try
                    {
                        await _courseManager.UpdateCourseAsync(course);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        var exists = await CourseExists(course.CourseId);
                        if (!exists)
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(course);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                // Get current admin user for the ViewBag
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                if (userId != 0)
                {
                    var currentUser = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == userId);
                    ViewBag.UserName = currentUser?.Username ?? "Admin";
                }

                var course = await _courseManager.GetCourseByIdAsync(id.Value);

                if (course == null)
                {
                    return NotFound();
                }

                return View(course);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _courseManager.DeleteCourseAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        private async Task<bool> CourseExists(int id)
        {
            var course = await _courseManager.GetCourseByIdAsync(id);
            return course != null;
        }
    }
}