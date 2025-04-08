using Microsoft.AspNetCore.Mvc;
using StudentInformationManagementSystem.Attributes;
using StudentInformationManagementSystem.Interfaces;
using StudentInformationManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Controllers
{
    [AuthorizeRoles("Student")]
    public class StudentCourseController : Controller
    {
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly IStudentRepository _studentRepository;

        public StudentCourseController(
            IStudentCourseRepository studentCourseRepository,
            IStudentRepository studentRepository)
        {
            _studentCourseRepository = studentCourseRepository;
            _studentRepository = studentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

            var student = await _studentRepository.GetByUserIdAsync(userId.Value);
            if (student == null)
                return NotFound();

            var enrollments = await _studentCourseRepository.GetByStudentIdAsync(student.StudentId);
            var viewModels = enrollments.Select(e => new CourseViewModel
            {
                CourseId = e.CourseId,
                CourseCode = e.Course.CourseCode,
                CourseName = e.Course.CourseName,
                Description = e.Course.Description,
                CreditHours = e.Course.CreditHours,
                Grade = e.Grade ?? "Not Graded",
                EnrollmentDate = e.EnrollmentDate
            }).ToList();  // Convert to List to avoid the IEnumerable error

            return View(viewModels);  // Now passing List<CourseViewModel>
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

            var student = await _studentRepository.GetByUserIdAsync(userId.Value);
            if (student == null)
                return NotFound();

            var enrollment = await _studentCourseRepository.GetByIdAsync(id);
            if (enrollment == null || enrollment.StudentId != student.StudentId)
                return NotFound();

            var viewModel = new CourseViewModel
            {
                CourseId = enrollment.CourseId,
                CourseCode = enrollment.Course.CourseCode,
                CourseName = enrollment.Course.CourseName,
                Description = enrollment.Course.Description,
                CreditHours = enrollment.Course.CreditHours,
                Grade = enrollment.Grade ?? "Not Graded",
                EnrollmentDate = enrollment.EnrollmentDate
            };

            return View(viewModel);
        }
    }
}
