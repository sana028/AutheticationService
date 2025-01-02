using System.Security.Claims;

namespace AuthenticationService.Interfaces
{
    public interface IJwtToken
    {
        string GenerateToken(string role, string email);

        ClaimsPrincipal ValidateToken(string token);
    }
}
