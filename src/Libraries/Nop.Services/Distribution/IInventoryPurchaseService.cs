using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Distribution;

namespace Nop.Services.Distribution
{
    public interface IInventoryPurchaseService
    {
        public IList<InventoryPurchase> GetInventoryPurchases(string status, IList<int> warehouseIds);
        public IList<InventoryPurchase> GetInventoryPurchasesByWarehouseIds(IList<int> warehouseIds);
        public IList<InventoryPurchase> GetInventoryPurchasesByIds(IList<int> ids);
        public InventoryPurchase GetInventoryPurchase(int id);
        public void DeleteInventoryPurchase(int id);
        public void AddInventoryPurchase(IList<InventoryPurchase> inventoryPurchases);
        public void UpdateInventoryPurchase(InventoryPurchase inventoryPurchase);
        public void UpdateInventoryPurchases(IList<InventoryPurchase> inventoryPurchase);
    }
}

