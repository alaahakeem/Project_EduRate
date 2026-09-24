namespace EduRate.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Builds a signed JWT carrying Name/Email/Role/ProfileId claims, exactly as the
        /// original AuthController did.
        /// </summary>
        (string Token, DateTime Expiration) GenerateToken(string userName, string email, string role, int profileId);
    }
}
