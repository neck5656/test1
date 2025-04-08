using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentInformationManagementSystem.Attributes;
using StudentInformationManagementSystem.Data;
using StudentInformationManagementSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Controllers
{
    [AuthorizeRoles("Admin")]
    public class StudentEnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentEnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentEnrollment
        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .ToListAsync();

            return View(enrollments);
        }

        // GET: StudentEnrollment/Assign
        public async Task<IActionResult> Assign()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: StudentEnrollment/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int studentId, int courseId)
        {
            if (ModelState.IsValid)
            {
                // Check if the course is active
                var course = await _context.Courses.FindAsync(courseId);
                if (course == null || !course.IsActive)
                {
                    ModelState.AddModelError("", "This course is not active and cannot be assigned.");
                    await PopulateDropdownsAsync();
                    return View();
                }

                // Check if student is already enrolled in this course
                var existingEnrollment = await _context.StudentCourses
                    .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

                if (existingEnrollment != null)
                {
                    if (existingEnrollment.IsActive)
                    {
                        ModelState.AddModelError("", "Student is already enrolled in this course.");
                        await PopulateDropdownsAsync();
                        return View();
                    }
                    else
                    {
                        // If enrollment exists but is inactive, reactivate it
                        existingEnrollment.IsActive = true;
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Course enrollment reactivated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }

                // Create new enrollment
                var studentCourse = new StudentCourse
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrollmentDate = System.DateTime.Now,
                    IsActive = true,
                    Grade = "Not graded"
                };

                _context.StudentCourses.Add(studentCourse);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Course assigned successfully!";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync();
            return View();
        }

        // GET: StudentEnrollment/Remove/5
        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .FirstOrDefaultAsync(sc => sc.StudentCourseId == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: StudentEnrollment/Remove/5
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var enrollment = await _context.StudentCourses.FindAsync(id);
            if (enrollment != null)
            {
                enrollment.IsActive = false;
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Enrollment removed successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Helper method to populate dropdowns
        private async Task PopulateDropdownsAsync()
        {
            // Get only students (users with RoleId = 3)
            var students = await _context.Students
                .Include(s => s.User)
                .Where(s => s.User.RoleId == 3 && s.User.IsActive)
                .ToListAsync();

            ViewBag.Students = new SelectList(students, "StudentId", "FullName");

            // Get only active courses
            var courses = await _context.Courses
                .Where(c => c.IsActive)
                .ToListAsync();

            ViewBag.Courses = new SelectList(courses, "CourseId", "CourseName");
        }
    }
}