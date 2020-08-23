using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Inventory;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the distribution model factory
    /// </summary>
    public interface IInventoryModelFactory
    {
        public InventorySearchModel PrepareProductInventorySearchModel(InventorySearchModel inventorySearchModel);

        public InventoryListModel PrepareProductInventoryListModel(InventorySearchModel inventorySearchModel);

        public InventoryPurchaseListModel PrepareInventoryPurchaseSearchListModel(InventoryPurchaseSearchModel inventoryPurchaseSearchModel);
    }
}
