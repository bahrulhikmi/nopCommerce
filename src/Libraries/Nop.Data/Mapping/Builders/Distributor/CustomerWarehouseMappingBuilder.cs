using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Core.Domain.Distribution;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Builders.Distributor
{
    /// <summary>
    /// Represents a customer warehouse mapping entity builder
    /// </summary>
    public class CustomerWarehouseMappingBuilder : NopEntityBuilder<CustomerWarehouseMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerWarehouseMapping), nameof(CustomerWarehouseMapping.CustomerId)))
                   .AsInt32().ForeignKey<Customer>().PrimaryKey()
               .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerWarehouseMapping), nameof(CustomerWarehouseMapping.WarehouseId)))
                   .AsInt32().ForeignKey<Warehouse>().PrimaryKey();
        }
    }
}
