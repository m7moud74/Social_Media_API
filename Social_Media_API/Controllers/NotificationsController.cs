using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media_API.Service.Notify_Service;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        //[SwaggerOperation(Summary = "Get my notifications", Description = "Retrieves all notifications for the currently logged-in user.")]
        //[SwaggerResponse(StatusCodes.Status200OK, "List of notifications returned successfully.")]
        //[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authorized.")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var list = await _notificationService.GetNotificationsForUserAsync(userId);
            return Ok(list);
        }

        [HttpPost("markAsRead/{id}")]
        //[SwaggerOperation(Summary = "Mark notification as read", Description = "Marks a specific notification as read for the logged-in user.")]
        //[SwaggerResponse(StatusCodes.Status200OK, "Notification marked as read successfully.")]
        //[SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authorized.")]
        //[SwaggerResponse(StatusCodes.Status404NotFound, "Notification not found.")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _notificationService.MarkAsReadAsync(id, userId);
            return Ok();
        }
    }
}
