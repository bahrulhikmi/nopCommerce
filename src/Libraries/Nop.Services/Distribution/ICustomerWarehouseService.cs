using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Distribution;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Distribution
{
    public interface ICustomerWarehouseService
    {
        void AddCustomerWarehouseMapping(int customerId, int warehouseId);
        void DeleteCustomerWarehouseMapping(int customerId, int warehouseId);

        void DeleteCustomerWarehouseMappings(int customerId);

        IList<Customer> GetCustomersManagingWarehouse(int warehouseId);
        IList<Warehouse> GetWarehousesMangedByCustomer(int customerId);

        public int[] GetCustomerWarehouseIds(int customerId);
    }
}
