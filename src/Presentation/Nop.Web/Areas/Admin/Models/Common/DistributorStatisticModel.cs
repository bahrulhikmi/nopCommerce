using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common
{
    public class DistributorStatisticModel : BaseNopModel
    {
        public int LowStockProductsCount { get; set; }
        public int InventoryChangesCount { get; set; }

        public int TotalStockCount { get; set; }
        public int UnpaidInventoriesPriceTotal { get; set; }        
    }
}

