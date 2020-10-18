using System;

namespace Nop.Core.Domain.Notifications
{
    public class Notification:BaseEntity
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
