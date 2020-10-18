using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Notifications;

namespace Nop.Web.Areas.Admin.Factories
{
    public interface INotificationModelFactory
    {
        NotificationModel PrepareNotificationModel(NotificationModel notificationModel);
        MyInboxModel PrepareMyInboxModel(MyInboxModel myInboxModel);
        NotificationListModel PrepareMyInboxListModel(MyInboxModel myInboxModel);
    }
}
