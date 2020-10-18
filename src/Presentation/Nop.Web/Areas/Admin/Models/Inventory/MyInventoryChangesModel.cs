using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class MyInventoryChangesModel
    {
        public InventoryChangeSearchModel InventoryChangeSearchModelForApproval { get; set; }

        public InventoryChangeSearchModel InventoryChangeSearchModelForApprovalHistory { get; set; }

}
}
