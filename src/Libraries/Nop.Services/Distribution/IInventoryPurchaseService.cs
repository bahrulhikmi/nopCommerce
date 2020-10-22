using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Distribution;

namespace Nop.Services.Distribution
{
    public interface IInventoryPurchaseService
    {
        public IList<InventoryPurchase> GetInventoryPurchases(IList<int> warehouseIds);
        public IList<InventoryPurchase> GetInventoryPurchasesWithoutPaymentId(IList<int> warehouseIds);

        public int GetUnpaidAmount(IList<int> warehouseIds);
        public IList<InventoryPurchase> GetInventoryPurchasesByIds(IList<int> ids);
        public IList<InventoryPurchase> GetInventoryPurchasesByPaymentId(int paymentId);
        public InventoryPurchase GetInventoryPurchase(int id);
        public void DeleteInventoryPurchase(int id);
        public void AddInventoryPurchase(IList<InventoryPurchase> inventoryPurchases);
        public void AddInventoryPurchase(InventoryPurchase inventoryPurchases);
        public void UpdateInventoryPurchase(InventoryPurchase inventoryPurchase);
        public void UpdateInventoryPurchases(IList<InventoryPurchase> inventoryPurchase);
        public void AddInventoryPurchasePayment(InventoryPurchasePayment inventoryPurchasePayment, IList<int> inventoryPurchaseIds);
        public IList<InventoryPurchasePayment> GetUnapprovedInventoryPurchasePayments();
        public IList<InventoryPurchasePayment> GetIncompleteInventoryPurchasePaymentsForCustomer(int customerId);
        public InventoryPurchasePayment CreateOrGetExistingPendingPurchasePayment(InventoryPurchasePayment inventoryPurchasePayment);
        public InventoryPurchasePayment GetInventoryPurchasePayment(int id);
        public IList<InventoryPurchasePayment> GetInventoryPurchasePayments(int[] id);
        public void UpdateInventoryPurchasePayment(InventoryPurchasePayment inventoryPurchasePayment);
        public void UpdateInventoryPurchasePayments(IList<InventoryPurchasePayment> inventoryPurchasePayments);
        public void DeleteInventoryPurchasePayment(InventoryPurchasePayment inventoryPurchasePayment);
        public IList<InventoryChangeView> GetAllInventoryChangeInProcess();

        public IList<InventoryChangeView> GetAllInventoryChangeInProcessForWarehouses(int[] warehouseIds);

        public int GetAllInventoryChangeInProcessForWarehousesCount(int[] warehouseIds);

        public IList<InventoryChangeView> GetAllInventoryChanges();

        public IList<InventoryChangeView> GetAllInventoryChangesForWarehouses(int[] warehouseIds);
        public void AddInventoryChanges(IList<InventoryChange> inventoryChanges);
        public InventoryChange GetInventoryChange(int id);
        void UpdateInventoryChange(InventoryChange inventoryChange);
    }
}

