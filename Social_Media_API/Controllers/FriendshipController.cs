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
    public class FriendshipController : ControllerBase
    {

        private readonly IFriendshipRpo _friendshipRepo;

        public FriendshipController(IFriendshipRpo friendshipRepo)
        {
            _friendshipRepo = friendshipRepo;
        }

        [HttpPost("send/{receiverId}")]
        public IActionResult SendRequest(string receiverId)
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

            return Ok("Friendship request sent.");
        }

       
        [HttpPost("accept/{id}")]
        public IActionResult AcceptRequest(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var friendship = _friendshipRepo.GetById(id);
            if (friendship == null || friendship.ReceiverId != userId)
                return NotFound();

            friendship.Status = FriendshipStatus.Accepted;
            _friendshipRepo.Update(id,friendship);
            _friendshipRepo.Save();

            return Ok("Friendship request accepted.");
        }

       
        [HttpPost("reject/{id}")]
        public IActionResult RejectRequest(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var friendship = _friendshipRepo.GetById(id);
            if (friendship == null || friendship.ReceiverId != userId)
                return NotFound();

            friendship.Status = FriendshipStatus.Rejected;
            _friendshipRepo.Update(id,friendship);
            _friendshipRepo.Save();

            return Ok("Friendship request rejected.");
        }

       
        [HttpGet("pending")]
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

