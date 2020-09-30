using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Distribution;
using Nop.Services.Catalog;
using Nop.Data;
using System.Linq;
using Nop.Services.Events;
using Nop.Core;
using LinqToDB.SchemaProvider;

namespace Nop.Services.Distribution
{
    public class InventoryPurchaseService : IInventoryPurchaseService
    {

        private readonly IRepository<InventoryPurchase> _inventoryPurchase;
        private readonly IEventPublisher _eventPublisher;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IRepository<InventoryPurchasePayment> _inventoryPurchasePayment;

        public InventoryPurchaseService(IRepository<InventoryPurchase> inventoryPurchase,
                                        IEventPublisher eventPublisher,
                                        IProductService productService,
                                        IWorkContext workContext,
                                        IStoreContext storeContext,
                                        IRepository<InventoryPurchasePayment> inventoryPurchasePayment)
        {
            _eventPublisher = eventPublisher;
            _inventoryPurchase = inventoryPurchase;
            _productService = productService;
            _workContext = workContext;
            _storeContext = storeContext;
            _inventoryPurchasePayment = inventoryPurchasePayment;
        }
        public IList<InventoryPurchase> GetInventoryPurchases(IList<int> warehouseIds)
        {
            var query = from purchase in _inventoryPurchase.Table
                        where warehouseIds.Contains(purchase.WarehouseId)
                        select purchase;

            return query.ToList();
        }

        public IList<InventoryPurchase> GetInventoryPurchasesWithoutPaymentId(IList<int> warehouseIds)
        {
            var query = from purchase in _inventoryPurchase.Table
                        where warehouseIds.Contains(purchase.WarehouseId) && purchase.PaymentId == 0
                        select purchase;

            return query.ToList();
        }




        public IList<InventoryPurchase> GetInventoryPurchasesByIds(IList<int> purchaseIds)
        {
            var query = from purchase in _inventoryPurchase.Table
                        where purchaseIds.Contains(purchase.Id)
                         && purchase.PaymentId == 0
                        select purchase;

            return query.ToList();
        }


        public void AddInventoryPurchase(InventoryPurchase inventoryPurchase)
        {

            _inventoryPurchase.Insert(inventoryPurchase);

            //event notification
            _eventPublisher.EntityInserted(inventoryPurchase);
        }

        public void AddInventoryPurchase(IList<InventoryPurchase> inventoryPurchases)
        {
            if (inventoryPurchases.Count == 0)
                return;

            _inventoryPurchase.Insert(inventoryPurchases);

            //event notification
            _eventPublisher.EntityInserted(inventoryPurchases[0]);
        }

        public void DeleteInventoryPurchase(int id)
        {
            if (id == 0)
                throw new ArgumentNullException("InventoryPurchase Id");
            var inventoryPurchase = new InventoryPurchase { Id = id };
            _inventoryPurchase.Delete(inventoryPurchase);

            //event notification
            _eventPublisher.EntityDeleted(inventoryPurchase);
        }

        public InventoryPurchase GetInventoryPurchase(int id)
        {
            return _inventoryPurchase.GetById(id);
        }

        public void UpdateInventoryPurchase(InventoryPurchase inventoryPurchase)
        {
            _inventoryPurchase.Update(inventoryPurchase);

            //event notification
            _eventPublisher.EntityUpdated(inventoryPurchase);
        }

        public void UpdateInventoryPurchases(IList<InventoryPurchase> inventoryPurchases)
        {
            _inventoryPurchase.Update(inventoryPurchases);

            foreach (var item in inventoryPurchases)
            {
                //event notification
                _eventPublisher.EntityUpdated(item);
            }
        }

        public void AddInventoryPurchasePayment(InventoryPurchasePayment inventoryPurchasePayment, IList<int> inventoryPurchaseIds)
        {
            if (inventoryPurchaseIds.Count == 0 || inventoryPurchasePayment.Id == 0)
                return;

            //check if it already have an existing payment id
            var existingPurchasesWithPaymentId = GetInventoryPurchasesByPaymentId(inventoryPurchasePayment.Id);
            
            var inventoryPurchases = GetInventoryPurchasesByIds(inventoryPurchaseIds);
            var total = existingPurchasesWithPaymentId.Select(x => x.PriceInclTax).Sum();
            foreach (var item in inventoryPurchases)
            {              
                if(!existingPurchasesWithPaymentId.Any(x=> x.Id == item.Id))
                {
                    item.PaymentId = inventoryPurchasePayment.Id;
                    total += item.PriceInclTax;
                }                    
            }

            inventoryPurchasePayment.Total = total;            
            
            UpdateInventoryPurchases(inventoryPurchases);

            UpdateInventoryPurchasePayment(inventoryPurchasePayment);
        }

        public IList<InventoryPurchase> GetInventoryPurchasesByPaymentId(int paymentId)
        {
            var query = from purchase in _inventoryPurchase.Table
                        where purchase.PaymentId == paymentId
                        select purchase;

            return query.ToList();
        }

        public IList<InventoryPurchasePayment> GetUnapprovedInventoryPurchasePayments()
        {
            var query = from purchase in _inventoryPurchasePayment.Table
                        where purchase.StatusId == (int)InventoryStatus.Processing
                        select purchase;

            return query.ToList();
        }

        public IList<InventoryPurchasePayment> GetIncompleteInventoryPurchasePaymentsForCustomer(int customerId)
        {
            var query = from purchase in _inventoryPurchasePayment.Table
                        where ((purchase.StatusId == (int)InventoryStatus.Pending)|| (purchase.StatusId == (int)InventoryStatus.Processing))
                        && purchase.CustomerId == customerId
                        select purchase;

            return query.ToList();
        }

        public InventoryPurchasePayment CreateOrGetExistingPendingPurchasePayment(InventoryPurchasePayment inventoryPurchasePayment)
        {
           var existing = (from purchase in _inventoryPurchasePayment.Table
                           where purchase.StatusId == (int)InventoryStatus.Pending
                           && purchase.CustomerId == inventoryPurchasePayment.CustomerId
                          select purchase).FirstOrDefault();
            if (existing != null)
                return existing;

            inventoryPurchasePayment.Status = InventoryStatus.Pending;
            _inventoryPurchasePayment.Insert(inventoryPurchasePayment);

            return inventoryPurchasePayment;
        }

        public InventoryPurchasePayment GetInventoryPurchasePayment(int id)
        {
            return _inventoryPurchasePayment.GetById(id);
        }

        public IList<InventoryPurchasePayment> GetInventoryPurchasePayments(int[] id)
        {
            var query = from purchase in _inventoryPurchasePayment.Table
                        where id.Contains(purchase.Id)
                        select purchase;

            return query.ToList();
        }

        public void UpdateInventoryPurchasePayment(InventoryPurchasePayment inventoryPurchasePayment)
        {
            _inventoryPurchasePayment.Update(inventoryPurchasePayment);

            _eventPublisher.EntityUpdated(inventoryPurchasePayment);
        }

        public void UpdateInventoryPurchasePayments(IList<InventoryPurchasePayment> inventoryPurchasePayments)
        {
            _inventoryPurchasePayment.Update(inventoryPurchasePayments);

            foreach (var item in inventoryPurchasePayments)
            {
                _eventPublisher.EntityUpdated(item);
            }           
        }

        public void DeleteInventoryPurchasePayment(InventoryPurchasePayment inventoryPurchasePayment)
        {
            _inventoryPurchasePayment.Delete(inventoryPurchasePayment);

            var inventoryPurchases = GetInventoryPurchasesByPaymentId(inventoryPurchasePayment.Id);            
            foreach (var item in inventoryPurchases)
            {
                item.PaymentId = 0;
            }

            UpdateInventoryPurchases(inventoryPurchases);

            _eventPublisher.EntityDeleted(inventoryPurchasePayment);
        }
    }
}

