using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NetCoreIntermediate.DbContextService;
using NetCoreIntermediate.Models;
using NuGet.Protocol.Plugins;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSignInsController : ControllerBase
    {
        private readonly AuthenticationDbContext AuthenticationContext;
        private readonly IConfiguration Configuration;

        public UserSignInsController(AuthenticationDbContext context, IConfiguration configuration)
        {
            AuthenticationContext = context;
            Configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserSignIn user)
        {

            var validateUser = AuthenticationContext.SignUpUsers
                   ?.Where(userData => userData.Email == user.Email && userData.Password == user.Password).FirstOrDefault();
            if (validateUser == null)
            {
                return Unauthorized("Invalid email and password");
            }
            else
            {
                var jwtSettings = Configuration.GetSection("JwtSettings");
                var issuer = jwtSettings["Issuer"];
                var audience = jwtSettings["Audience"];
                var secretKey = jwtSettings["SecretKey"];
                var generateToken = JwtAuthenticationFactory.GetInstance(secretKey, issuer, audience);
                var token = generateToken.GenerateToken(validateUser.Role, validateUser.Email);
                if (!string.IsNullOrEmpty(token))
                {
                    return Ok(new{ Token = token });
                }
            }
            return BadRequest();
        }
    }
}
