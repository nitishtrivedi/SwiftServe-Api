using SwiftServe_API.Models;
using SwiftServe_API.Repositories;

namespace SwiftServe_API.Services
{
    public class NotificationService
    {
        private readonly IRepository<Notification> _repo;

        public NotificationService(IRepository<Notification> repo)
        {
            _repo = repo;
        }

        public async Task Create(int userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message
            };

            await _repo.Add(notification);
            await _repo.Save();
        }

        public List<Notification> GetUserNotifications(int userId)
        {
            return _repo.GetAll()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public async Task MarkAsRead(int id)
        {
            var notification = await _repo.GetById(id);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                await _repo.Save();
            }
        }
    }
}
