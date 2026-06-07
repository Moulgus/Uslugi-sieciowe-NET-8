using BlogCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromBody] LoginModel userLogin)
        {
            var user = Authenticate(userLogin);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var token = GenerateToken(user);
            return Ok(token);
        }

        private string GenerateToken(LoginModel user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing.");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing.");
            var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is missing.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var token = new JwtSecurityToken(
                jwtIssuer,
                jwtAudience,
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static LoginModel? Authenticate(LoginModel userLogin)
        {
            return UserConstants.Users.FirstOrDefault(user =>
                user.Username.Equals(userLogin.Username, StringComparison.OrdinalIgnoreCase)
                && user.Password == userLogin.Password);
        }
    }
}
