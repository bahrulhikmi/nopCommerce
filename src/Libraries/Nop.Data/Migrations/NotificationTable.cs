using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;
using Nop.Core.Domain.Notifications;

namespace Nop.Data.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/10/10 11:24:16:2551771", "Notification Table")]
    public class NotificationTable : AutoReversingMigration
    {

        private readonly IMigrationManager _migrationManager;

        public NotificationTable(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<Notification>(Create);
        }
    
    }
}
