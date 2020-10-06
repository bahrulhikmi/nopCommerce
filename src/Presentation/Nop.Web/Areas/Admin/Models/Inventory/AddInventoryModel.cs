using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Storage.Blob.Protocol;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class AddInventoryModel
    {
        public AddInventoryModel()
        {
            AvailableWarehouses = new List<SelectListItem>();
            InventoryAdditions = new List<InventoryAddition>();
            InventorySearchModel = new InventorySearchModel();
        }

        [NopResourceDisplayName("Admin.Common.AdditionalFee")]
        public int AdditionalFee { get; set; }

        [NopResourceDisplayName("Admin.Common.Note")]
        public string Note { get; set; }


        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Warehouse")]
        public int WarehouseId { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
        public IList<InventoryAddition> InventoryAdditions { get; set; }

        public InventorySearchModel InventorySearchModel { get; set; }
    }

    public class InventoryAddition {
        public int Id { get; set; }
        public int Amount { get; set; }
    }
}
 