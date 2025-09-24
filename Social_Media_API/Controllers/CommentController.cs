using Microsoft.AspNetCore.Mvc;
using Social_Media_API.Dto.Account_DTO;
using Social_Media_API.Dto.Comment_DTO;
using Social_Media_API.Model;
using Social_Media_API.Reposatory.Comment_Repo;
using Social_Media_API.Reposatory.Post_Repo;
using Social_Media_API.Service.Notify_Service;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepo commentRepo;
        private readonly INotificationService _notificationService;
        private readonly IPostRepo postRepo;

        public CommentController(ICommentRepo commentRepo, INotificationService notificationService, IPostRepo postRepo)
        {
            this.commentRepo = commentRepo;
            _notificationService = notificationService;
            this.postRepo = postRepo;
        }

   
        [HttpGet]
        //[SwaggerOperation(
        //    Summary = "Get all comments",
        //    Description = "Retrieves all comments with user and post info."
        //)]
        //[SwaggerResponse(200, "Comments retrieved successfully", typeof(IEnumerable<CommentDto>))]
        [SwaggerResponse(204, "No comments found")]
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

       
        [HttpPost]
        //[SwaggerOperation(
        //    Summary = "Add a new comment",
        //    Description = "Creates a new comment on a specific post and notifies the post owner."
        //)]
        //[SwaggerResponse(201, "Comment created successfully", typeof(CommentDto))]
        //[SwaggerResponse(401, "Unauthorized")]
        //[SwaggerResponse(404, "Post not found")]
        public async Task<IActionResult> AddComment(CreateCommentDto commentDto)
        {
            var post = postRepo.GetById(commentDto.PostId);
            if (post == null)
                return NotFound("Post not found");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var userName = User.Identity?.Name;

            var comment = new Comment
            {
                PostId = commentDto.PostId,
                UserId = userId,
                Content = commentDto.Content,
                CreatedAt = DateTime.Now
            };

            commentRepo.Create(comment);
            commentRepo.Save();

            if (post.UserId != userId)
            {
                await _notificationService.NotifyAsync(
                    post.UserId,
                    "PostCommented",
                    $"{userName} commented on your post: {commentDto.Content}"
                );
            }

            var result = new CommentDto
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                CommentUserDto = new UserDto
                {
                    UserId = userId,
                    UserName = userName,
                    ProfilePictureUrl = User.FindFirst("profilePicture")?.Value
                }
            };

            return CreatedAtAction(nameof(GetAllComments), new { id = comment.CommentId }, result);
        }

       
        [HttpPut("{id}")]
        //[SwaggerOperation(
        //    Summary = "Update a comment",
        //    Description = "Allows the user to update their own comment."
        //)]
        //[SwaggerResponse(200, "Comment updated successfully", typeof(CommentDto))]
        //[SwaggerResponse(401, "Unauthorized")]
        //[SwaggerResponse(403, "User cannot update another user's comment")]
        //[SwaggerResponse(404, "Comment not found")]
        public IActionResult UpdateComment(int id, CreateCommentDto commentDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var comment = commentRepo.GetById(id);
            if (comment == null)
                return NotFound("Comment not found");

            if (comment.UserId != userId)
                return Forbid("You can only update your own comment");

            comment.Content = commentDto.Content;
            comment.CreatedAt = DateTime.Now;

            commentRepo.Update(id, comment);
            commentRepo.Save();

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

        
        [HttpDelete("{id}")]
        //[SwaggerOperation(
        //    Summary = "Delete a comment",
        //    Description = "Allows the user to delete their own comment."
        //)]
        //[SwaggerResponse(204, "Comment deleted successfully")]
        //[SwaggerResponse(401, "Unauthorized")]
        //[SwaggerResponse(403, "User cannot delete another user's comment")]
        //[SwaggerResponse(404, "Comment not found")]
        public IActionResult DeleteComment(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var comment = commentRepo.GetById(id);
            if (comment == null)
                return NotFound("Comment not found");

            if (comment.UserId != userId)
                return Forbid("You can only delete your own comment");

            commentRepo.Delete(id);
            commentRepo.Save();

            return NoContent();
        }
    }
}
