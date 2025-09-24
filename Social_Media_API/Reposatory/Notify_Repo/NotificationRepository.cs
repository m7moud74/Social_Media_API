using Microsoft.EntityFrameworkCore;
using Social_Media_API.Data;
using Social_Media_API.Model;

namespace Social_Media_API.Reposatory.Notify_Repo
{
    public class NotificationRepository:INotificationRepository
    {
        private readonly AppDbContext _db;
        public NotificationRepository(AppDbContext _db)
        {
            this._db = _db;
        }

        public async Task CreateAsync(Notification notification)
        {

            await _db.Notifications.AddAsync(notification);
        }
        public async Task<List<Notification>> GetAllByUserAsync(string UserId)
        {

            return await _db.Notifications.Where(n => n.UserId == UserId).ToListAsync();
        }
        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _db.Notifications.FindAsync(id);
        }
        public async Task MarkAsReadAsync(int id)
        {
            var n = await GetByIdAsync(id);
            if (n != null)
            {
                n.IsRead = true;
            }
        }
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
