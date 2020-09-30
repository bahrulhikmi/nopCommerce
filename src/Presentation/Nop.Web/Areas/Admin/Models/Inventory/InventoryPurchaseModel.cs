using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Distribution;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class InventoryPurchaseModel: BaseNopEntityModel
    {
        public int ProductId { get; set; }

        public int PaymentId { get; set; }

        public string ProductName { get; set; }

        public string Sku { get; set; }

        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; }        

        public int Quantity { get; set; }

        public decimal PriceInclTax { get; set; }

        public decimal PriceExclTax { get; set; }
      
        public decimal TotalDiscount { get; set; }

        public DateTime? PurchasedDate { get; set; }

        public string Note { get; set; }

        public string Status { get; set; }

        public int StatusId { get; set; }
    }
}
