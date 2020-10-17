using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;
using Nop.Core.Domain.Distribution;

namespace Nop.Data.Migrations
{

    [SkipMigrationOnUpdate]
    [NopMigration("2020/10/06 11:24:16:2551771", "Inventory Change")]
    public class InventoryChangeTable: AutoReversingMigration
    {

        private readonly IMigrationManager _migrationManager;

        public InventoryChangeTable(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<InventoryChange>(Create);
        }
    }
}
