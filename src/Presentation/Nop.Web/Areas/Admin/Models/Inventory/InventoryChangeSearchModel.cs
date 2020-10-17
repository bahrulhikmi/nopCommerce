using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class InventoryChangeSearchModel : BaseSearchModel
    {
        public bool ApprovalVisible { get; set; }
    }
}
