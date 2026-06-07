using BlogCMS.Interfaces;
using BlogCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IRepository<Post> _postRepository;

        public PostsController(IRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Post>>> GetAllPosts()
        {
            var posts = await _postRepository.GetAllAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Post>> GetPostById(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newPostId = await _postRepository.AddAsync(post);
            return CreatedAtAction(nameof(GetPostById), new { id = newPostId }, post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _postRepository.UpdateAsync(post);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var result = await _postRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
