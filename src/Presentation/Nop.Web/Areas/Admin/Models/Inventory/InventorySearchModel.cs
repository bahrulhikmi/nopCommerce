using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
   public class InventorySearchModel : BaseSearchModel
    {
        public InventorySearchModel()
        {

            Sku = string.Empty;
            ProductName = string.Empty;
        }
      
        [NopResourceDisplayName("Admin.Catalog.Inventories.ProductName")]
        public string ProductName { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Inventories.SKU")]
        public string Sku { get; set; }

        public int CustomerId { get; set; }

        public bool MultipleWarehouses { get; set; }
    }
}
