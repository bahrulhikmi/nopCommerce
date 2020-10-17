using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Inventory;

using Nop.Core.Domain.Distribution;
namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the distribution model factory
    /// </summary>
    public interface IInventoryModelFactory
    {
        public ManageInventoryPurchasePaymentModel PrepareManageInventoryModel(ManageInventoryPurchasePaymentModel inventorySearchModel);

        public InventoryListModel PrepareProductInventoryListModel(InventorySearchModel inventorySearchModel);

        public InventoryPurchasePaymentListModel PrepareInventoryPurchasePaymentSearchListModel(InventoryPurchasePaymentSearchModel inventoryPurchasePaymentSearchModel);

        public InventoryPurchaseListModel PrepareInventoryPurchaseSearchListModel(InventoryPurchaseSearchModel inventoryPurchaseSearchModel);

        public InventoryPurchaseListModel PrepareIncludedInPaymentInventoryPurchaseSearchListModel(InventoryPurchaseSearchModel inventoryPurchaseSearchModel);

        public InventoryPurchaseListModel PrepareNotInPaymentInventoryPurchaseSearchListModel(InventoryPurchaseSearchModel inventoryPurchaseSearchModel);

        public InventoryPurchasePaymentModel PrepareUpdateInventoryPurchasePaymentModel(InventoryPurchasePayment inventoryPurchasePaymentModel);

        public AddInventoryModel PrepareAddInventoryModel(AddInventoryModel inventoryModel);

        public AllInventoryModel PrepareAllInventoryModel(AllInventoryModel inventoryModel);

        public InventoryChangeListModel PrepareInventoryChangesInProcessListModel(InventoryChangeSearchModel inventoryChangeListModel);
    }
}

