using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class InventoryPurchasePaymentSearchModel : BaseSearchModel
    {        
        public string WarehouseName { get; set; }
        public bool UnapprovedOnly { get; set; }
        public bool MyPaymentOnly { get; set; }
    }
}
