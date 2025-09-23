using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_API.Dto;
using Social_Media_API.Model;
using Social_Media_API.Reposatory;
using Social_Media_API.Service;
using System.Security.Claims;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class PostController : ControllerBase
    {
        private readonly IGenaricRepo<Post> _genaricRepo;
        private readonly IPostRepo _postRepo;
        private readonly INotificationService _notificationService;

        public PostController(
            IPostRepo postRepo,
            IGenaricRepo<Post> genaricRepo,
            INotificationService notificationService)
        {
            _postRepo = postRepo;
            _genaricRepo = genaricRepo;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult GetAllPosts()
        {
            var postsContent = _postRepo.GetWithIncludes();
            var posts = postsContent.Select(post => new PostDto
            {
                PostId = post.PostId,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                ImageUrl = post.ImageUrl,
                PostUserDto = new UserDto
                {
                    UserId = post.User.Id,
                    UserName = post.User.UserName
                },
                Comments = post.Comments.Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    PostId=c.PostId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    CommentUserDto = new UserDto
                    {
                        UserId = c.User.Id,
                        UserName = c.User.UserName
                    }
                }).ToList(),
                Likes = post.Likes.Select(l => new LikeDto
                {
                    PostId = l.PostId,
                    CreateAt=l.CreatAt,
                    LikeUserDto = new UserDto
                    {
                        UserId = l.User.Id,
                        UserName = l.User.UserName,
                        ProfilePictureUrl=l.User.ProfilePictureUrl

                    }
                }).ToList()
            }).ToList();

            return Ok(posts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto postDto)
        {
            if (postDto == null)
                return BadRequest("Can't publish empty post.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;
            var profilePicture = User.FindFirst("profilePicture")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var post = new Post
            {
               
                Content = postDto.Content,
                ImageUrl = postDto.ImageUrl,
                CreatedAt = DateTime.Now,
                UserId = userId
            };

            _genaricRepo.Create(post);
            _genaricRepo.Save();

            await _notificationService.NotifyAsync(
                userId,
                "PostCreated",
                $"Hi {userName}, your post was created successfully."
            );

            var postReadDto = new PostReadDto
            {
                PostId = post.PostId,
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
                CommentCount = 0,
               
            };

            return CreatedAtAction(nameof(GetAllPosts), postReadDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CreatePostDto post)
        {
            var existingPost = _genaricRepo.GetById(id);
            if (post == null || existingPost == null)
                return BadRequest("Post is null.");

            existingPost.Content = post.Content;
            existingPost.ImageUrl = post.ImageUrl;

            _genaricRepo.Update(id, existingPost);
            _genaricRepo.Save();

            return Ok("Post updated successfully.");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingPost = _genaricRepo.GetById(id);
            if (existingPost == null)
                return NotFound("Post not found.");

            _genaricRepo.Delete(id);
            _genaricRepo.Save();

            return NoContent();
        }
    }
}
