using BlogCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Admins")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminEndPoint()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            return Ok($"Hi you are an {currentUser.Role}");
        }

        private LoginModel? GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return null;
            }

            var userClaims = identity.Claims;
            return new LoginModel
            {
                Username = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
                Role = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value ?? string.Empty
            };
        }
    }
}
