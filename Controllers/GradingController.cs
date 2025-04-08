using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInformationManagementSystem.Attributes;
using StudentInformationManagementSystem.Data;
using StudentInformationManagementSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Controllers
{
    [AuthorizeRoles("Faculty")]
    public class GradingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Grading
        public async Task<IActionResult> Index()
        {
            // Get all courses for the faculty to grade
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        // GET: Grading/CourseStudents/5
        public async Task<IActionResult> CourseStudents(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            // Get all students enrolled in this course
            var enrollments = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Where(sc => sc.CourseId == id && sc.IsActive)
                .ToListAsync();

            ViewBag.CourseName = course.CourseName;
            ViewBag.CourseId = course.CourseId;

            return View(enrollments);
        }

        // GET: Grading/SubmitGrade/5
        public async Task<IActionResult> SubmitGrade(int id)
        {
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

        // POST: Grading/SubmitGrade/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitGrade(int id, string grade)
        {
            var enrollment = await _context.StudentCourses.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            try
            {
                enrollment.Grade = grade;
                _context.Update(enrollment);
                await _context.SaveChangesAsync();

                // This is where you could implement the Observer pattern
                // to notify students of grade changes

                return RedirectToAction(nameof(CourseStudents), new { id = enrollment.CourseId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentCourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool StudentCourseExists(int id)
        {
            return _context.StudentCourses.Any(e => e.StudentCourseId == id);
        }
    }
}
