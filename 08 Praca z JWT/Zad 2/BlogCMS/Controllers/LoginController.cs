using BlogCMS.Data;
using BlogCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly BlogDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(BlogDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginModel userLogin)
        {
            var user = await Authenticate(userLogin);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var token = GenerateToken(user);
            return Ok(token);
        }

        private string GenerateToken(UserAccount user)
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

        private async Task<UserAccount?> Authenticate(LoginModel userLogin)
        {
            return await _context.Users.FirstOrDefaultAsync(user =>
                user.Username.ToLower() == userLogin.Username.ToLower()
                && user.Password == userLogin.Password);
        }
    }
}
