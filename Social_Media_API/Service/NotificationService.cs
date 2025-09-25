using Microsoft.AspNetCore.SignalR;
using Social_Media_API.Model;
using Social_Media_API.Reposatory;

namespace Social_Media_API.Service
{
    public class NotificationService:INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(INotificationRepository repo, IHubContext<NotificationHub> hubContext)
        {
            _repo = repo;
            _hubContext = hubContext;
        }

        public async Task NotifyAsync(string toUserId, string type, string message)
        {
            var notification = new Notification
            {
                UserId = toUserId,
                Type = type,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _repo.CreateAsync(notification);
            await _repo.SaveChangesAsync();

           
            await _hubContext.Clients.User(toUserId).SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                type = notification.Type,
                message = notification.Message,
                createdAt = notification.CreatedAt
            });
        }

        public async Task<List<Notification>> GetNotificationsForUserAsync(string userId)
        {
            return await _repo.GetAllByUserAsync(userId);
        }

        public async Task MarkAsReadAsync(int notificationId, string currentUserId)
        {
            var n = await _repo.GetByIdAsync(notificationId);
            if (n == null) throw new Exception("Not found");
            if (n.UserId != currentUserId) throw new UnauthorizedAccessException();
            n.IsRead = true;
            await _repo.SaveChangesAsync();
        }
    }
}
