using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Distribution;

namespace Nop.Services.Distribution
{
    public interface IInventoryPurchaseService
    {
        public IList<InventoryPurchase> GetInventoryPurchases(string status);
    }
}
