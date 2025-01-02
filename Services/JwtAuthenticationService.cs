using AuthenticationService.Interfaces;
using AuthenticationService.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetCoreIntermediate.Services
{
    public class JwtAuthenticationService : IJwtToken
    {
        private readonly string SecretKey;
        private readonly string Issuer;
        private readonly string Audience;

        public JwtAuthenticationService(string secretKey, string issuer, string audience)
        {
            SecretKey = secretKey;
            Issuer = issuer;
            Audience = audience;
        }


        public string GenerateToken(string role, string email)
        {
            var claims = new List<Claim>{
                new Claim("Email",JwtRegisteredClaimNames.Email,email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               Issuer,
               Audience,
                claims,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey))
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
    }
}

