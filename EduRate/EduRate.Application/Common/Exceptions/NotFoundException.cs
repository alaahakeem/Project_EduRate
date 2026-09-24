namespace EduRate.Application.Common.Exceptions
{
    /// <summary>Mirrors the original controllers' NotFound(...) responses. Mapped to HTTP 404.</summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
