using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Notifications;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class NotificationsController : BaseAdminController
    {
        INotificationModelFactory _notificationModelFactory;
        IWorkContext _workContect;
        public NotificationsController(INotificationModelFactory notificationModelFactory,
                                    IWorkContext workContext)
        {
            _notificationModelFactory = notificationModelFactory;
            _workContect = workContext;
        }
        public IActionResult MyInbox()
        {
            var model = _notificationModelFactory.PrepareMyInboxModel(new MyInboxModel());
            return View(model);
        }

        [HttpPost]
        public IActionResult GetAllUserNotifications(MyInboxModel searchModel)
        {
            var model = _notificationModelFactory.PrepareMyInboxListModel(searchModel);
            return Json(model);
        }
    }
}
