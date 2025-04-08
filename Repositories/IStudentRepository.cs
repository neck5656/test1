using StudentInformationManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> GetByIdAsync(int id);
        Task<Student> GetByUserIdAsync(int userId);
        Task<IEnumerable<Student>> GetAllAsync();
        Task<int> CreateAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(int id);
    }
}