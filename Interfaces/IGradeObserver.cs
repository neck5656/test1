using StudentInformationManagementSystem.Models;

namespace StudentInformationManagementSystem.Interfaces
{
    public interface IGradeObserver
    {
        void Update(StudentCourse studentCourse);
    }
}