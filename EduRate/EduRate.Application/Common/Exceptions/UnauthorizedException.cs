namespace EduRate.Application.Common.Exceptions
{
    /// <summary>Mirrors the original controllers' Unauthorized(...) responses (e.g. missing/invalid ProfileId claim). Mapped to HTTP 401.</summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
