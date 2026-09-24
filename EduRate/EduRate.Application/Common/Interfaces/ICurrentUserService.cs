namespace EduRate.Application.Common.Interfaces
{
    /// <summary>
    /// Reads the current caller's identity out of the JWT claims.
    /// Replaces the "GetXIdFromToken()" helper methods that used to be duplicated
    /// in almost every controller.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>The "ProfileId" claim (Student/Teacher/Center row id), if present and valid.</summary>
        int? ProfileId { get; }

        /// <summary>The user's role claim (Student, Teacher, Center, Admin), if any.</summary>
        string? Role { get; }

        /// <summary>The ASP.NET Identity user id (string Guid), if any.</summary>
        string? UserId { get; }

        /// <summary>The user's email claim, if any.</summary>
        string? Email { get; }

        bool IsInRole(string role);
    }
}
