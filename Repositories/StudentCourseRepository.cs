using StudentInformationManagementSystem.Data;
using StudentInformationManagementSystem.Interfaces;
using StudentInformationManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace StudentInformationManagementSystem.Repositories
{
    public class StudentCourseRepository : IStudentCourseRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StudentCourseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<StudentCourse>> GetByStudentIdAsync(int studentId)
        {
            return await _dbContext.StudentCourses
                .Include(sc => sc.Course)
                .Where(sc => sc.StudentId == studentId && sc.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentCourse>> GetByCourseIdAsync(int courseId)
        {
            return await _dbContext.StudentCourses
                .Include(sc => sc.Student)
                .ThenInclude(s => s.User)
                .Where(sc => sc.CourseId == courseId && sc.IsActive)
                .ToListAsync();
        }

        public async Task<StudentCourse> GetByIdAsync(int id)
        {
            return await _dbContext.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .FirstOrDefaultAsync(sc => sc.StudentCourseId == id);
        }

        public async Task UpdateAsync(StudentCourse studentCourse)
        {
            _dbContext.StudentCourses.Update(studentCourse);
            await _dbContext.SaveChangesAsync();
        }
    }
}