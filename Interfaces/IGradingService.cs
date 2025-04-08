// Interfaces/IGradingService.cs
namespace StudentInformationManagementSystem.Interfaces
{
    public interface IGradingService
    {
        void Attach(IGradeObserver observer);
        void Detach(IGradeObserver observer);
        Task SubmitGradeAsync(int studentCourseId, string grade);
    }
}
