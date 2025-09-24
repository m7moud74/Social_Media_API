using Social_Media_API.Model;

namespace Social_Media_API.Service.Notify_Service
{
    public interface INotificationService
    {
  public Task NotifyAsync(string toUserId, string type, string message);
  public Task<List<Notification>> GetNotificationsForUserAsync(string userId);
  public Task MarkAsReadAsync(int notificationId, string currentUserId);
    }
}
