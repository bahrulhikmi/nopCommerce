using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Notifications;
using Nop.Data;
using Nop.Services.Events;
using Nop.Services.Notifications;

namespace Nop.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notification;
        private readonly IEventPublisher _eventPublisher;
        public NotificationService(IRepository<Notification> notification,
                                    IEventPublisher eventPublisher)
        {
            _notification = notification;
            _eventPublisher = eventPublisher;
        }

        public int CountUnreadNotificationForUser(int userId)
        {
            return _notification.Table.Count(n => n.ForUserId == userId && n.Unread);
        }

        public void CreateNotification(string message, int userId)
        {
            var notification = new Notification()
            {
                Unread = true,
                Body = message,
                ForUserId= userId
            };
            CreateNotification(notification);
        }

        public void CreateNotification(Notification notification)
        {
            _notification.Insert(notification);
            
            _eventPublisher.EntityInserted(notification);            
        }

        public IList<Notification> ReadAllNotificationsForUser(int userId)
        {
            return _notification.Table.Where(n => n.ForUserId == userId).ToList();
        }
    }
}
