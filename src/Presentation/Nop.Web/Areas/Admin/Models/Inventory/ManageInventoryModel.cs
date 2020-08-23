using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class ManageInventoryModel
    {
        public InventorySearchModel InventorySearchModel { get; set; }
        public InventoryPurchaseSearchModel InventoryPurchaseSearchModel { get; set; }
    }
}
