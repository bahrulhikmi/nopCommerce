using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Inventory
{
    public class InventoryPurchasePaymentModel: BaseNopEntityModel
    {
        public InventoryPurchasePaymentModel()
        {
            AvailableInventoryPurchaseSearchModel = new InventoryPurchaseSearchModel();
            IncludedInventoryPurchaseSearchModel = new InventoryPurchaseSearchModel();
            AddedInventoryPurchaseSearchModel = new List<InventoryPurchaseModel>();
        }

        public InventoryPurchaseSearchModel AvailableInventoryPurchaseSearchModel { get; set; }
        public InventoryPurchaseSearchModel IncludedInventoryPurchaseSearchModel { get; set; }
        public List<InventoryPurchaseModel> AddedInventoryPurchaseSearchModel { get;set;}

        public int PaymentId { get; set; }


        [NopResourceDisplayName("Admin.Common.User")]
        public string User { get; set; }

        [NopResourceDisplayName("Admin.Common.Note")]    
        public string Note { get; set; }

        [NopResourceDisplayName("Order.TransferReceipt")]
        [UIHint("DownloadSimple")]        
        public int DownloadId { get; set; }

        [NopResourceDisplayName("Admin.Common.Download")]
        public Guid DownloadGuid { get; set; }

        [NopResourceDisplayName("Admin.Common.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        public bool AllowApproval { get; set; }

        public decimal Total { get; set; }

        public bool ReadOnly { get; set; }       
        
        public string Status { get; set; }

        public int StatusId { get; set; }

        public bool AllowCancel { get; set; }

    }
}
