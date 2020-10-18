using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Notifications;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Notifications;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    public class NotificationModelFactory : INotificationModelFactory
    {
        INotificationService _notificationService;
        IWorkContext _workContext;
        public NotificationModelFactory(INotificationService notificationService,
            IWorkContext workContext)
        {
            _notificationService = notificationService;
            _workContext = workContext;
        }
        public NotificationListModel PrepareMyInboxListModel(MyInboxModel myInboxModel)
        {
            var notifications = _notificationService.ReadAllNotificationsForUser(_workContext.CurrentCustomer.Id).ToPagedList(myInboxModel);            

            var model = new NotificationListModel().PrepareToGrid(myInboxModel, notifications, () =>
            {
                return notifications.Select(notification =>
                {
                    var notificationModel = notification.ToModel<NotificationModel>();                  
                    return notificationModel;
                }
                );
            });

            return model;
        }

        public MyInboxModel PrepareMyInboxModel(MyInboxModel myInboxModel)
        {
            return myInboxModel;
        }

        public NotificationModel PrepareNotificationModel(NotificationModel notificationModel)
        {
            return notificationModel;
        }
    }
}
