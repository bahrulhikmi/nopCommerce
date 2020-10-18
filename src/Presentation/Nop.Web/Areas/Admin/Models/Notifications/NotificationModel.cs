using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Notifications
{
    public class NotificationModel:BaseNopEntityModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Unread { get; set; }
        public DateTime CreationDate { get; set; }
        public int FromUserId { get; set; }
        public int ForUserId { get; set; }
        public int ForRoleId { get; set; }
        public DateTime ExpiredDate { get; set; }

        public string RelatedAction { get; set; }
    }
}
