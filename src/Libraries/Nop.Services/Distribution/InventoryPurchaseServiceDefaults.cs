using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Distribution
{
   public class InventoryPurchaseServiceDefaults
    {
        public static class Status
        {
            public const string Pending = "PENDING";
            public const string Closed = "CLOSED";
        }

        public const string PurchaseRecordDefaultAmount = "Inventories.PurchaseRecord.DefaultAmount";
    }
}
