using FluentMigrator;
using Nop.Core.Domain.Distribution;

namespace Nop.Data.Migrations
{

    [SkipMigrationOnUpdate]
    [NopMigration("2020/08/09 11:24:16:2551771", "Customer Ware House Mapping")]
    public class CustomerWarehouse : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public CustomerWarehouse(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
        public override void Up()
        {
            _migrationManager.BuildTable<CustomerWarehouseMapping>(Create);
        }
    }
}
