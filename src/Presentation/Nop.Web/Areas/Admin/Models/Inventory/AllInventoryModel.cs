using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class AllInventoryModel: BaseNopEntityModel
    {
        public InventorySearchModel InventorySearchModel { get; set; }
        public InventoryChangeSearchModel InventoryChangeSearchModel { get; set; }
    }
}
