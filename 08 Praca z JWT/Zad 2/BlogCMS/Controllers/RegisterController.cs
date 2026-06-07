using BlogCMS.Data;
using BlogCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RegisterController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public RegisterController(BlogDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usernameExists = await _context.Users.AnyAsync(user =>
                user.Username.ToLower() == registerModel.Username.ToLower());
            if (usernameExists)
            {
                return Conflict("Username already exists");
            }

            var user = new UserAccount
            {
                Username = registerModel.Username,
                Password = registerModel.Password,
                Role = "Admin"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { id = user.Id }, new
            {
                user.Id,
                user.Username,
                user.Role
            });
        }
    }
}
