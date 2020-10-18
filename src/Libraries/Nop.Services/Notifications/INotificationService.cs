using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Notifications;

namespace Nop.Services.Notifications
{
    public interface INotificationService
    {
        void CreateNotification(Notification notification);
        void CreateNotification(string message, int userId);

        IList<Notification> ReadAllNotificationsForUser(int userId);

        int CountUnreadNotificationForUser(int userId);
    }
}
