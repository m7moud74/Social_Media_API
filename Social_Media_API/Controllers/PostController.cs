using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Social_Media_API.Dto;
using Social_Media_API.Model;
using Social_Media_API.Reposatory;
using System.Security.Claims;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IGenaricRepo<Post> genaricRepo;

        private readonly IPostRepo postRepo; 

        public PostController(IPostRepo postRepo,IGenaricRepo<Post> genaricRepo)
        {
            this.postRepo = postRepo;
            this.genaricRepo = genaricRepo;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAllPosts()
        {
            var postscontnet= postRepo.GetWithIncludes();
            List<PostDto> posts = new List<PostDto>();
            foreach (var post in postscontnet)
            {
                
                PostDto postDto = new PostDto
                {
                    PostId = post.PostId,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    ImageUrl = post.ImageUrl,
                    PostUserDto = new UserDto
                    {
                        UserId = post.User.Id,
                        UserName = post.User.UserName,
                       
                    },
                    Comments = post.Comments.Select(c => new CommentDto
                    {
                        CommentId = c.CommentId,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        CommentUserDto = new UserDto
                        {
                            UserId = c.User.Id,
                            UserName = c.User.UserName,
                           
                        }
                    }).ToList(),
                    Likes = post.Likes.Select(l => new LikeDto
                    {
                        LikeId = l.LikeId,
                        
                        PostId = l.PostId,
                        LikeUserDto = new UserDto
                        {
                            UserId = l.User.Id,
                            UserName = l.User.UserName,
                          
                        }
                    }).ToList()
                };
                posts.Add(postDto);
            }
            
            return Ok(posts);
        }
        [HttpPost]
        [Authorize]
        public IActionResult CreatePost([FromBody] CreatePostDto postDto)
        {
            if (postDto == null)
            {
                return BadRequest("Can't Publich Empty Post");
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;
            var profilePicture = User.FindFirst("profilePicture")?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var post = new Post
            {
                Content = postDto.Content,
                ImageUrl = postDto.ImageUrl,
                CreatedAt = DateTime.Now,
                UserId = userId,
               

            };

            genaricRepo.Create(post);
           
            genaricRepo.Save();
            Console.WriteLine(post.PostId);

            var postReadDto = new PostReadDto
            {
                Id = post.PostId,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                AuthorName = userName,
                AuthorPic = profilePicture,
                PostUserDto = new UserDto
                {
                    UserId = userId,
                    UserName = userName
                },
                LikeCount = 0,
                CommentCount = 0
            };

            return CreatedAtAction(nameof(GetAllPosts), new { id = post.PostId }, postReadDto);
        }
        [HttpPut("{id}")]
        [Authorize]

        public IActionResult Update (int id,CreatePostDto post)
        {
            var existingPost = genaricRepo.GetById(id);
            if (post == null||existingPost==null)
                return BadRequest("Post is null.");
            existingPost.Content = post.Content;
            existingPost.ImageUrl = post.ImageUrl;
            genaricRepo.Update(id, existingPost);
            genaricRepo.Save();

            return Ok();
        }
        [HttpDelete("{id}")]
        [Authorize]

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
