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
    [Authorize]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly IGenaricRepo<Like> genaricRepo;
        private readonly ILikeRepo likeRepo;
        private readonly IPostRepo postRepo;

        public LikeController(IGenaricRepo<Like> genaricRepo, ILikeRepo likeRepo, IPostRepo postRepo)
        {
            this.genaricRepo = genaricRepo;
            this.likeRepo = likeRepo;
            this.postRepo = postRepo;
        }
        [HttpGet]
        public IActionResult GetAllLikes()
        {
            var allLikes = likeRepo.GetWithIncludes();

            var likes = allLikes.Select(like => new LikeDto
            {
                LikeId = like.LikeId,
                PostId = like.PostId,
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
        public IActionResult AddLike(CreateLikeDto likeDto)
        {
            var post = genaricRepo.GetById(likeDto.PostId);
            if (post == null)
                return NotFound("Post not found");
            if (likeDto == null) return BadRequest();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var userName = User.Identity?.Name;
            var profilePicture = User.FindFirst("profilePicture")?.Value;
            var like = new Like
            {
                PostId = likeDto.PostId,
                UserId = userId
            };

            genaricRepo.Create(like);
            genaricRepo.Save();
            var result = new LikeDto
            {
                LikeId = like.LikeId,
                PostId = like.PostId,
                LikeUserDto = new UserDto
                {
                    UserId = userId,
                    UserName = User.Identity?.Name,
                    ProfilePictureUrl = User.FindFirst("profilePicture")?.Value
                }
            };

            return CreatedAtAction(nameof(GetAllLikes), new { id = like.LikeId }, result);
        }
        [HttpDelete]
        public IActionResult DeleteLike(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var like = genaricRepo.GetById(id);
            if (like == null)
                return NotFound("Like not found");

            if (like.UserId != userId)
                return Forbid("You can only delete your own like");

            genaricRepo.Delete(id);
            genaricRepo.Save();

            return NoContent();
        }
    }
}

