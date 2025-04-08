using StudentInformationManagementSystem.Interfaces;

namespace StudentInformationManagementSystem.Services
{
    public class GradingService : IGradingService
    {
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly List<IGradeObserver> _observers = new List<IGradeObserver>();

        public GradingService(IStudentCourseRepository studentCourseRepository)
        {
            _studentCourseRepository = studentCourseRepository;
        }

        public void Attach(IGradeObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IGradeObserver observer)
        {
            _observers.Remove(observer);
        }

        public async Task SubmitGradeAsync(int studentCourseId, string grade)
        {
            var studentCourse = await _studentCourseRepository.GetByIdAsync(studentCourseId);
            if (studentCourse != null)
            {
                studentCourse.Grade = grade;
                await _studentCourseRepository.UpdateAsync(studentCourse);

                // Notify all observers
                foreach (var observer in _observers)
                {
                    observer.Update(studentCourse);
                }
            }
        }
    }
}