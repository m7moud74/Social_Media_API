using Microsoft.AspNetCore.Mvc;
using Social_Media_API.Dto;
using Social_Media_API.Dto.FriendShip_DTO;
using Social_Media_API.Model;
using Social_Media_API.Reposatory.FriendShip_Repo;
using Social_Media_API.Reposatory.Notify_Repo;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace Social_Media_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipRpo _friendshipRepo;
        private readonly INotificationRepository _notificationRepo;

        public FriendshipController(IFriendshipRpo friendshipRepo, INotificationRepository notificationRepo)
        {
            _friendshipRepo = friendshipRepo;
            _notificationRepo = notificationRepo;
        }

        [HttpPost("send/{receiverId}")]
        //[SwaggerOperation(Summary = "Send a friendship request", Description = "Allows a user to send a friendship request to another user.")]
        //[SwaggerResponse(200, "Friendship request sent.")]
        //[SwaggerResponse(400, "Invalid request.")]
        //[SwaggerResponse(401, "Unauthorized.")]
        public async Task<IActionResult> SendRequest(string receiverId)
        {
            var requesterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (requesterId == null) return Unauthorized();

            if (requesterId == receiverId)
                return BadRequest("You cannot send a request to yourself.");

            if (_friendshipRepo.Exists(requesterId, receiverId))
                return BadRequest("Friendship request already exists.");

            var friendship = new Friendship
            {
                RequesterId = requesterId,
                ReceiverId = receiverId,
                Status = FriendshipStatus.Pending,
            };

            _friendshipRepo.Create(friendship);
            _friendshipRepo.Save();

            var notification = new Notification
            {
                UserId = receiverId,
                Type = "FriendRequest",
                Message = $"{User.Identity?.Name} sent you a friend request."
            };
            await _notificationRepo.CreateAsync(notification);
            await _notificationRepo.SaveChangesAsync();

            return Ok("Friendship request sent.");
        }

        [HttpPost("accept/{id}")]
        //[SwaggerOperation(Summary = "Accept a friendship request", Description = "Allows a user to accept a friendship request.")]
        //[SwaggerResponse(200, "Friendship request accepted.")]
        //[SwaggerResponse(401, "Unauthorized.")]
        //[SwaggerResponse(404, "Friendship request not found.")]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var friendship = _friendshipRepo.GetById(id);
            if (friendship == null || friendship.ReceiverId != userId)
                return NotFound();

            friendship.Status = FriendshipStatus.Accepted;
            _friendshipRepo.Update(id, friendship);
            _friendshipRepo.Save();

            var notification = new Notification
            {
                UserId = friendship.RequesterId,
                Type = "FriendRequestAccepted",
                Message = $"{User.Identity?.Name} accepted your friend request."
            };
            await _notificationRepo.CreateAsync(notification);
            await _notificationRepo.SaveChangesAsync();

            return Ok("Friendship request accepted.");
        }

        [HttpPost("reject/{id}")]
        //[SwaggerOperation(Summary = "Reject a friendship request", Description = "Allows a user to reject a friendship request.")]
        //[SwaggerResponse(200, "Friendship request rejected.")]
        //[SwaggerResponse(401, "Unauthorized.")]
        //[SwaggerResponse(404, "Friendship request not found.")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var friendship = _friendshipRepo.GetById(id);
            if (friendship == null || friendship.ReceiverId != userId)
                return NotFound();

            friendship.Status = FriendshipStatus.Rejected;
            _friendshipRepo.Update(id, friendship);
            _friendshipRepo.Save();

            var notification = new Notification
            {
                UserId = friendship.RequesterId,
                Type = "FriendRequestRejected",
                Message = $"{User.Identity?.Name} rejected your friend request."
            };
            await _notificationRepo.CreateAsync(notification);
            await _notificationRepo.SaveChangesAsync();

            return Ok("Friendship request rejected.");
        }

        [HttpGet("pending")]
        //[SwaggerOperation(Summary = "Get pending friendship requests", Description = "Fetches all pending friendship requests for the current user.")]
        //[SwaggerResponse(200, "List of pending requests.")]
        //[SwaggerResponse(401, "Unauthorized.")]
        public IActionResult GetPendingRequests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var requests = _friendshipRepo.GetPendingRequests(userId)
                .Select(f => new FriendshipDto
                {
                    FriendshipId = f.FriendshipId,
                    RequesterId = f.RequesterId,
                    ReceiverId = f.ReceiverId,
                    RequesterName = f.Requester?.UserName,
                    ReceiverName = f.Receiver?.UserName,
                    Status = f.Status
                })
                .ToList();

            return Ok(requests);
        }

        [HttpGet("friends")]
        //[SwaggerOperation(Summary = "Get list of friends", Description = "Fetches all accepted friendships for the current user.")]
        //[SwaggerResponse(200, "List of friends.")]
        //[SwaggerResponse(401, "Unauthorized.")]
        public IActionResult GetFriends()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var friends = _friendshipRepo.GetFriends(userId)
                .Select(f => new FriendshipDto
                {
                    FriendshipId = f.FriendshipId,
                    RequesterId = f.RequesterId,
                    ReceiverId = f.ReceiverId,
                    RequesterName = f.Requester?.UserName,
                    ReceiverName = f.Receiver?.UserName,
                    Status = f.Status
                })
                .ToList();

            return Ok(friends);
        }
    }
}
