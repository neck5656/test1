using StudentInformationManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentInformationManagementSystem.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> GetByIdAsync(int id);
        Task<IEnumerable<Course>> GetAllAsync();
        Task<int> CreateAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(int id);
    }
}