namespace EduRate.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task SendToStudentAsync(int studentId, string title, string message);
        Task SendToTeacherAsync(int teacherId, string title, string message);
        Task SendToCenterAsync(int centerId, string title, string message);
    }
}
