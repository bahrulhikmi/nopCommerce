using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Distribution
{
    public class InventoryChange: BaseEntity
    {
        public DateTime DateUtc { get; set; }

        public int StockQuantityChange { get; set; }

        public string Description { get; set; }

        public int StatusId { get; set; }

        public int InventoryId { get; set; }

        public string Note { get; set; }                     

        public int CreatedByUserId { get; set; }

        public int LastStatusChangeByUserId { get; set; }

        public InventoryChangeStatus Status { get => (InventoryChangeStatus)StatusId; set => StatusId = (int)value; }

    }
}
