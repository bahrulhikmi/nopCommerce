﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class InventoryPurchaseModel: BaseNopEntityModel
    {
        public int ProductId { get; set; }

        public int WarehouseId { get; set; }

        public int Quantity { get; set; }

        public long PriceInclTax { get; set; }

        public long PriceExclTax { get; set; }
      
        public long TotalDiscount { get; set; }

        public DateTime? PurchasedDate { get; set; }

        public string Note { get; set; }

        public string Status { get; set; }
    }
}
