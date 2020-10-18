using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class InventoryChangeModel: BaseNopEntityModel
    {
        public DateTime DateUtc { get; set; }

        public int StockQuantityChange { get; set; }

        public string WareHouseName { get; set; }

        public string ProductName { get; set; }

        public string ProductSKU { get; set; }
        public string Description { get; set; }

        public int StatusId { get; set; }

        public int InventoryId { get; set; }

        public string Note { get; set; }

        public int CreatedByUserId { get; set; }

        public string CreatedByUser { get; set; }

        public int LastStatusChangeByUserId { get; set; }

        public string LastStatusChangeByUser { get; set; }

        public string Status { get; set; }
    }
}
