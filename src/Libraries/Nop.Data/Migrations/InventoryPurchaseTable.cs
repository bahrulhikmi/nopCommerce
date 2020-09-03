using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;
using Nop.Core.Domain.Distribution;

namespace Nop.Data.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/08/17 11:24:16:2551771", "Inventory Purchase Table")]
    public class InventoryPurchaseTable : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public InventoryPurchaseTable(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<InventoryPurchase>(Create);
        }
    }
}
