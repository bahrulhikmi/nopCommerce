using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Distribution
{
    class InventoryChangeHistory
    {
        public DateTime DateUtc { get; set; }

        public int AmountChanged { get; set; }

        public string Description { get; set; }
    }
}
