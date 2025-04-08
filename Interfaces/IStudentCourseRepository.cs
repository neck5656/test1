// Interfaces/IStudentCourseRepository.cs
using StudentInformationManagementSystem.Models;

namespace StudentInformationManagementSystem.Interfaces
{
    public interface IStudentCourseRepository
    {
        Task<IEnumerable<StudentCourse>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<StudentCourse>> GetByCourseIdAsync(int courseId);
        Task<StudentCourse> GetByIdAsync(int id);
        Task UpdateAsync(StudentCourse studentCourse);
    }
}
