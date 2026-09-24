using System.Threading.Tasks;

namespace EduRate.Services
{
    public interface INotificationService
    {
        Task SendToStudentAsync(int studentId, string title, string message);
        Task SendToTeacherAsync(int teacherId, string title, string message);
        Task SendToCenterAsync(int centerId, string title, string message);
    }
}