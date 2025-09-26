using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Social_Media_API.Dto;
using Social_Media_API.Model;
using Social_Media_API.Reposatory;
using Social_Media_API.Service;
using System.Security.Claims;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeRepo likeRepo;
        private readonly IPostRepo postRepo;
        private readonly INotificationService _notificationService;

        public LikeController(ILikeRepo likeRepo, IPostRepo postRepo, INotificationService notificationService)
        {
            this.likeRepo = likeRepo;
            this.postRepo = postRepo;
            _notificationService = notificationService;
        }

        [HttpGet]
        //[SwaggerOperation(Summary = "Get all likes", Description = "Fetches all likes with related user and post info.")]
        //[SwaggerResponse(200, "List of likes returned successfully.")]
        //[SwaggerResponse(204, "No likes found.")]
        public IActionResult GetAllLikes(int postid)
        {
            var allLikes = likeRepo.GetWithIncludes().Where(l => l.PostId == postid);

            var likes = allLikes.Select(like => new LikeDto
            {
                PostId = like.PostId,
                CreateAt = like.CreatAt,
                Reaction = like.Reaction,
                LikeUserDto = new UserDto
                {
                    UserId = like.UserId,
                    UserName = like.User.UserName,
                    ProfilePictureUrl = like.User.ProfilePictureUrl
                }
            }).ToList();

            if (!likes.Any())
                return NoContent();

            return Ok(likes);
        }

        [HttpPost]
        //[SwaggerOperation(Summary = "Add a like to a post", Description = "Allows an authenticated user to like a specific post.")]
        //[SwaggerResponse(201, "Like created successfully.")]
        //[SwaggerResponse(400, "Invalid like data.")]
        //[SwaggerResponse(401, "Unauthorized.")]
        //[SwaggerResponse(404, "Post not found.")]
        public async Task<IActionResult> AddLike(CreateLikeDto likeDto)
        {
            var post = postRepo.GetById(likeDto.PostId);
            if (post == null)
                return NotFound("Post not found");

            if (likeDto == null) return BadRequest();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var userName = User.Identity?.Name;

            var like = new Like
            {
                PostId = likeDto.PostId,
                UserId = userId,
                Reaction = likeDto.Reaction,
                CreatAt = DateTime.Now
            };

            likeRepo.Create(like);
            likeRepo.Save();

            if (post.UserId != userId)
            {
                await _notificationService.NotifyAsync(
                    post.UserId,
                    "PostLiked",
                    $"{userName} liked your post with React {like.Reaction}."
                );
            }

            var result = new LikeDto
            {
                PostId = like.PostId,
                CreateAt = like.CreatAt,
                Reaction = likeDto.Reaction,
                LikeUserDto = new UserDto
                {
                    UserId = userId,
                    UserName = userName,
                    ProfilePictureUrl = User.FindFirst("profilePicture")?.Value
                }
            };

            return CreatedAtAction(nameof(GetAllLikes), new { id = like.LikeId }, result);
        }

        [HttpDelete("{id}")]
        //[SwaggerOperation(Summary = "Delete a like", Description = "Allows an authenticated user to remove their like from a post.")]
        //[SwaggerResponse(204, "Like deleted successfully.")]
        //[SwaggerResponse(401, "Unauthorized.")]
        //[SwaggerResponse(403, "User not allowed to delete this like.")]
        //[SwaggerResponse(404, "Like not found.")]
        public IActionResult DeleteLike(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var like = likeRepo.GetById(id);
            if (like == null)
                return NotFound("Like not found");

            if (like.UserId != userId)
                return Forbid("You can only delete your own like");

            likeRepo.Delete(id);
            likeRepo.Save();

            return NoContent();
        }
    }
}


