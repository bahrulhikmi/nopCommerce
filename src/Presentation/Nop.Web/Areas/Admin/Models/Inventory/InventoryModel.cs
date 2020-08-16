using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class InventoryModel : BaseNopEntityModel
    {
        public int ProductId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Inventories.ProductName")]
        public string ProductName { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Inventories.Sku")]
        public string Sku { get; set; }
        public int WarehouseId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Inventories.WarehouseName")]
        public string WarehouseName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Inventories.StockQty")]
        public int StockQuantity { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Inventories.ReservedQty")]
        public int ReservedQuantity { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Inventories.PlannedQty")]
        public int PlannedQuantity { get; set; }
    }
}
