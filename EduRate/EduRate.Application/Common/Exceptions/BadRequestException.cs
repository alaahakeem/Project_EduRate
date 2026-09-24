namespace EduRate.Application.Common.Exceptions
{
    /// <summary>Mirrors the original controllers' BadRequest(...) responses for business-rule violations. Mapped to HTTP 400.</summary>
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
