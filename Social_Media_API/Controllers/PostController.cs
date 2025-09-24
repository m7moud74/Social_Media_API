using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media_API.Dto;
using Social_Media_API.Dto.Account_DTO;
using Social_Media_API.Dto.Comment_DTO;
using Social_Media_API.Dto.Like_DTO;
using Social_Media_API.Dto.Post_DTO;
using Social_Media_API.Model;
using Social_Media_API.Reposatory.Post_Repo;
using Social_Media_API.Service.Notify_Service;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostRepo _postRepo;
        private readonly INotificationService _notificationService;

        public PostController(IPostRepo postRepo, INotificationService notificationService)
        {
            _postRepo = postRepo;
            _notificationService = notificationService;
        }

        [HttpGet]
        //[SwaggerOperation(Summary = "Get all posts", Description = "Retrieves all posts including user, comments, and likes.")]
        //[SwaggerResponse(StatusCodes.Status200OK, "List of posts retrieved successfully.")]
        //[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authorized.")]
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
                    UserName = post.User.UserName,
                    ProfilePictureUrl = post.User.ProfilePictureUrl
                },
                Comments = post.Comments.Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    PostId = c.PostId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    CommentUserDto = new UserDto
                    {
                        UserId = c.User.Id,
                        UserName = c.User.UserName,
                        ProfilePictureUrl = c.User.ProfilePictureUrl
                    }
                }).ToList(),
                Likes = post.Likes.Select(l => new LikeDto
                {
                    PostId = l.PostId,
                    CreateAt = l.CreatAt,
                    Reaction = l.Reaction,
                    LikeUserDto = new UserDto
                    {
                        UserId = l.User.Id,
                        UserName = l.User.UserName,
                        ProfilePictureUrl = l.User.ProfilePictureUrl
                    }
                }).ToList()
            }).ToList();

            return Ok(posts);
        }

        [HttpPost("create")]
        //[SwaggerOperation(Summary = "Create a post", Description = "Creates a new post with optional image.")]
        //[SwaggerResponse(StatusCodes.Status201Created, "Post created successfully.")]
        //[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid post data.")]
        //[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authorized.")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto postDto)
        {
            if (postDto == null)
                return BadRequest("Can't publish empty post.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;
            var profilePicture = User.FindFirst("profilePicture")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            string imageUrl = null;
            if (postDto.ImageUrl != null && postDto.ImageUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(postDto.ImageUrl.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postDto.ImageUrl.CopyToAsync(stream);
                }

                imageUrl = $"/images/{uniqueFileName}";
            }

            var post = new Post
            {
                Content = postDto.Content,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.Now,
                UserId = userId
            };

            _postRepo.Create(post);
            _postRepo.Save();

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
                CommentCount = 0
            };

            return CreatedAtAction(nameof(GetAllPosts), postReadDto);
        }

        [HttpPut("{id}")]
        //[SwaggerOperation(Summary = "Update a post", Description = "Updates the content and/or image of an existing post.")]
        //[SwaggerResponse(StatusCodes.Status200OK, "Post updated successfully.")]
        //[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid post data.")]
        //[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authorized.")]
        //[SwaggerResponse(StatusCodes.Status404NotFound, "Post not found.")]
        public async Task<IActionResult> Update(int id, [FromForm] CreatePostDto postDto)
        {
            var existingPost = _postRepo.GetById(id);
            if (postDto == null || existingPost == null)
                return BadRequest("Post is null.");

            existingPost.Content = postDto.Content;

            if (postDto.ImageUrl != null && postDto.ImageUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(postDto.ImageUrl.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postDto.ImageUrl.CopyToAsync(stream);
                }

                existingPost.ImageUrl = $"/images/{uniqueFileName}";
            }

            _postRepo.Update(id, existingPost);
            _postRepo.Save();

            return Ok("Post updated successfully.");
        }

        [HttpDelete("{id}")]
        //[SwaggerOperation(Summary = "Delete a post", Description = "Deletes a post by its ID.")]
        //[SwaggerResponse(StatusCodes.Status204NoContent, "Post deleted successfully.")]
        //[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authorized.")]
        //[SwaggerResponse(StatusCodes.Status404NotFound, "Post not found.")]
        public IActionResult Delete(int id)
        {
            var existingPost = _postRepo.GetById(id);
            if (existingPost == null)
                return NotFound("Post not found.");

            _postRepo.Delete(id);
            _postRepo.Save();

            return NoContent();
        }
    }
}
