using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Social_Media_API.Model;
using Social_Media_API.Reposatory;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IGenaricRepo<Post> genaricRepo;

        public PostController(IGenaricRepo<Post> genaricRepo )
        {
            this.genaricRepo = genaricRepo;
        }
        [HttpGet]
        public IActionResult GetAllPosts()
        {
            var posts = genaricRepo.GetAll();
            return Ok(posts);
        }
        [HttpPost]
        public IActionResult CreatePost([FromBody] Post post)
        {
            if (post == null)
            {
                return BadRequest("Post is null.");
            }
            genaricRepo.Create(post);
            genaricRepo.Save();
            return CreatedAtAction(nameof(GetAllPosts), new { id = post.PostId }, post);
        }
        [HttpPut("{id}")]

        public IActionResult Update (int id,Post post)
        {
            var existingPost = genaricRepo.GetById(id);
            if (post == null||existingPost==null)
                return BadRequest("Post is null.");
            genaricRepo.Update(id, post);

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingPost = genaricRepo.GetById(id);
            if (existingPost == null)
                return NotFound("Post not found.");
            genaricRepo.Delete(id);
            genaricRepo.Save();
            return NoContent();
        }
    }
}
