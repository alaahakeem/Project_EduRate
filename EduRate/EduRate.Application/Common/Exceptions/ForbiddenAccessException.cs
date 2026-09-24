namespace EduRate.Application.Common.Exceptions
{
    /// <summary>Mirrors the original controllers' Forbid(...) responses (e.g. acting on another teacher's session). Mapped to HTTP 403.</summary>
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message) : base(message) { }
    }
}
