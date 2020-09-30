using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Distribution;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class InventoryPurchaseSearchModel : BaseSearchModel
    {
        public int CustomerId { get; set; }

        public bool MultipleWarehouses { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Inventories.WarehouseName")]
        public string WarehouseName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Inventories.UnapprovedOnly")]
        public bool UnapprovedOnly { get; set; }

        public int PaymentId { get; set; }

    }
}
