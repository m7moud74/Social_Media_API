using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media_API.Dto;
using Social_Media_API.Model;
using Social_Media_API.Reposatory;
using System.Security.Claims;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepo commentRepo;
        private readonly IGenaricRepo<Post> genaricPostRepo;
        private readonly IGenaricRepo<Comment> genaricCommentRepo;

        public CommentController(ICommentRepo commentRepo, IGenaricRepo<Post> genaricPostRepo, IGenaricRepo<Comment> genaricCommentRepo)
        {
            this.commentRepo = commentRepo;
            this.genaricPostRepo = genaricPostRepo;
            this.genaricCommentRepo = genaricCommentRepo;
        }

        // GET: api/comment
        [HttpGet]
        public IActionResult GetAllComments()
        {
            var allComments = commentRepo.GetWithInclude();

            var comments = allComments.Select(comment => new CommentDto
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                CommentUserDto = new UserDto
                {
                    UserId = comment.UserId,
                    UserName = comment.User.UserName,
                    ProfilePictureUrl = comment.User.ProfilePictureUrl
                }
            }).ToList();

            if (!comments.Any())
                return NoContent();

            return Ok(comments);
        }

        // POST: api/comment
        [HttpPost]
        public IActionResult AddComment(CreateCommentDto commentDto)
        {
            var post = genaricPostRepo.GetById(commentDto.PostId);
            if (post == null)
                return NotFound("Post not found");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var comment = new Comment
            {
                PostId = commentDto.PostId,
                UserId = userId,
                Content = commentDto.Content,
                CreatedAt = DateTime.Now
            };

            genaricCommentRepo.Create(comment);
            genaricCommentRepo.Save();

            var result = new CommentDto
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                CommentUserDto = new UserDto
                {
                    UserId = userId,
                    UserName = User.Identity?.Name,
                    ProfilePictureUrl = User.FindFirst("profilePicture")?.Value
                }
            };

            return CreatedAtAction(nameof(GetAllComments), new { id = comment.CommentId }, result);
        }

        // PUT: api/comment/5
        [HttpPut("{id}")]
        public IActionResult UpdateComment(int id, CreateCommentDto commentDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var comment = genaricCommentRepo.GetById(id);
            if (comment == null)
                return NotFound("Comment not found");

            if (comment.UserId != userId)
                return Forbid("You can only update your own comment");

            comment.Content = commentDto.Content;
            comment.CreatedAt = DateTime.Now;

            genaricCommentRepo.Update(id,comment);
            genaricCommentRepo.Save();

            return Ok(new CommentDto
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                CommentUserDto = new UserDto
                {
                    UserId = userId,
                    UserName = User.Identity?.Name,
                    ProfilePictureUrl = User.FindFirst("profilePicture")?.Value
                }
            });
        }

        // DELETE: api/comment/5
        [HttpDelete("{id}")]
        public IActionResult DeleteComment(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var comment = genaricCommentRepo.GetById(id);
            if (comment == null)
                return NotFound("Comment not found");

            if (comment.UserId != userId)
                return Forbid("You can only delete your own comment");

            genaricCommentRepo.Delete(id);
            genaricCommentRepo.Save();

            return NoContent();
        }
    }
}
