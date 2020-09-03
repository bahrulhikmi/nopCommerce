﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Distribution
{
    public class InventoryPurchase: BaseEntity
    {
        public int ProductId { get; set; }

        public int WarehouseId { get; set; }

        public int Quantity { get; set; }

        public decimal PriceInclTax { get; set; }

        public decimal PriceExclTax { get; set; }

        public decimal TotalDiscount { get; set; }

        public DateTime? PurchasedDate { get; set; }

        public string Note { get; set; }

        public string Status { get; set; }
    }
}
