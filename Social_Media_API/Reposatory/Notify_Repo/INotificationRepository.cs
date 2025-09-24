using Social_Media_API.Model;

namespace Social_Media_API.Reposatory.Notify_Repo
{
    public interface INotificationRepository
    {
public Task CreateAsync(Notification notification);
public Task<List<Notification>> GetAllByUserAsync(string userId);
public Task<Notification?> GetByIdAsync(int id);
public Task MarkAsReadAsync(int id);
public Task SaveChangesAsync();
    }
}
