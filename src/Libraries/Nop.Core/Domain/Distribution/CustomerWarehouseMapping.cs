using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Distribution
{
    /// <summary>
    /// Represents Customer Warehouse Mapping class
    /// </summary>
   public class CustomerWarehouseMapping: BaseEntity
    {
        public int CustomerId { get; set; }
        public int WarehouseId { get; set; }
    }
}
