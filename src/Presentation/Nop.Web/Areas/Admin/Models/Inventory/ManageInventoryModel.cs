using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class ManageInventoryModel
    {
        public ManageInventoryModel()
        {
            InventorySearchModel = new InventorySearchModel();
            InventoryPurchaseSearchModel = new InventoryPurchaseSearchModel();
        }

        [NopResourceDisplayName("Admin.Catalog.Inventories.Qty")]
        public int Amount { get; set; }
        public int CustomerId { get; set; }
        public bool MultipleWarehouses { get; set; }    
        public InventorySearchModel InventorySearchModel { get; set; }
        public InventoryPurchaseSearchModel InventoryPurchaseSearchModel { get; set; }
    }
}
