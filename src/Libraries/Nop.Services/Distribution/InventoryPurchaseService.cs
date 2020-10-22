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
using Nop.Core.Domain.Catalog;
using Microsoft.AspNetCore.ResponseCaching;
using Nop.Core.Domain.Shipping;

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
        private readonly IRepository<InventoryChange> _inventoryChange;
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseInventory;
        private readonly IRepository<Product> _product;
        private readonly IRepository<Warehouse> _warehouse;

        public InventoryPurchaseService(IRepository<InventoryPurchase> inventoryPurchase,
                                        IEventPublisher eventPublisher,
                                        IProductService productService,
                                        IWorkContext workContext,
                                        IStoreContext storeContext,
                                        IRepository<InventoryPurchasePayment> inventoryPurchasePayment,
                                        IRepository<InventoryChange> inventoryChange,
                                        IRepository<ProductWarehouseInventory> productWarehouseInventory,
                                       IRepository<Product> product,
                                        IRepository<Warehouse> warehouse)
        {
            _eventPublisher = eventPublisher;
            _inventoryPurchase = inventoryPurchase;
            _productService = productService;
            _workContext = workContext;
            _storeContext = storeContext;
            _inventoryPurchasePayment = inventoryPurchasePayment;
            _inventoryChange = inventoryChange;
            _productWarehouseInventory = productWarehouseInventory;
            _product = product;
            _warehouse = warehouse;
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

            foreach (var item in inventoryPurchases)
            {
                _eventPublisher.EntityInserted(item);
            }
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
                if (!existingPurchasesWithPaymentId.Any(x => x.Id == item.Id))
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
                        where ((purchase.StatusId == (int)InventoryStatus.Pending) || (purchase.StatusId == (int)InventoryStatus.Processing))
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

        public IList<InventoryChangeView> GetAllInventoryChangeInProcess()
        {
            var query = from change in _inventoryChange.Table
                        join inventory in _productWarehouseInventory.Table on change.InventoryId equals inventory.Id
                        join product in _product.Table on inventory.ProductId equals product.Id
                        join warehouse in _warehouse.Table on inventory.WarehouseId equals warehouse.Id                        
                        where change.StatusId == (int)InventoryChangeStatus.Processing
                        select new InventoryChangeView()
                        {
                            Id = change.Id,
                            CreatedByUserId = change.CreatedByUserId,
                            DateUtc = change.DateUtc,
                            Description = change.Description,
                            InventoryId = change.InventoryId,
                            LastStatusChangeByUserId = change.LastStatusChangeByUserId,
                            Note = change.Note,
                            ProductName = product.Name,
                            ProductSKU = product.Sku,
                            StatusId = change.StatusId,
                            StockQuantityChange = change.StockQuantityChange,
                            WareHouseName = warehouse.Name
                        };

            return query.ToList();
        }

        public void AddInventoryChanges(IList<InventoryChange> inventoryChanges)
        {
            if (inventoryChanges.Count == 0)
                return;

            _inventoryChange.Insert(inventoryChanges);

            foreach (var item in inventoryChanges)
            {
                _eventPublisher.EntityInserted(item);
            }
        }

        public IList<InventoryChangeView> GetAllInventoryChangeInProcessForWarehouses(int[] warehouseIds)
        {
            var query = from change in _inventoryChange.Table
                        join inventory in _productWarehouseInventory.Table on change.InventoryId equals inventory.Id
                        join product in _product.Table on inventory.ProductId equals product.Id
                        join warehouse in _warehouse.Table on inventory.WarehouseId equals warehouse.Id
                        where warehouseIds.Contains(inventory.WarehouseId)
                        && change.StatusId == (int)InventoryChangeStatus.Processing
                        select new InventoryChangeView()
                        {
                            Id = change.Id,
                            CreatedByUserId = change.CreatedByUserId,
                            DateUtc = change.DateUtc,
                            Description = change.Description,
                            InventoryId = change.InventoryId,
                            LastStatusChangeByUserId = change.LastStatusChangeByUserId,
                            Note = change.Note,
                            ProductName = product.Name,
                            ProductSKU = product.Sku,
                            StatusId = change.StatusId,
                            StockQuantityChange = change.StockQuantityChange,
                            WareHouseName = warehouse.Name
                        };

            return query.ToList();
        }

        public InventoryChange GetInventoryChange(int id)
        {
            var query = from change in _inventoryChange.Table                      
                        where change.Id == id
                        select change;

            return query.FirstOrDefault();
        }

        public IList<InventoryChangeView> GetAllInventoryChanges()
        {
            var query = from change in _inventoryChange.Table
                        join inventory in _productWarehouseInventory.Table on change.InventoryId equals inventory.Id
                        join product in _product.Table on inventory.ProductId equals product.Id
                        join warehouse in _warehouse.Table on inventory.WarehouseId equals warehouse.Id
                        select new InventoryChangeView()
                        {
                            Id = change.Id,
                            CreatedByUserId = change.CreatedByUserId,
                            DateUtc = change.DateUtc,
                            Description = change.Description,
                            InventoryId = change.InventoryId,
                            LastStatusChangeByUserId = change.LastStatusChangeByUserId,
                            Note = change.Note,
                            ProductName = product.Name,
                            ProductSKU = product.Sku,
                            StatusId = change.StatusId,
                            StockQuantityChange = change.StockQuantityChange,
                            WareHouseName = warehouse.Name
                        };

            return query.ToList();
        }

        public IList<InventoryChangeView> GetAllInventoryChangesForWarehouses(int[] warehouseIds)
        {
            var query = from change in _inventoryChange.Table
                        join inventory in _productWarehouseInventory.Table on change.InventoryId equals inventory.Id
                        join product in _product.Table on inventory.ProductId equals product.Id
                        join warehouse in _warehouse.Table on inventory.WarehouseId equals warehouse.Id
                        where warehouseIds.Contains(inventory.WarehouseId)
                        select new InventoryChangeView() {
                            Id = change.Id,
                            CreatedByUserId = change.CreatedByUserId,
                            DateUtc = change.DateUtc,
                            Description = change.Description,
                            InventoryId = change.InventoryId,
                            LastStatusChangeByUserId = change.LastStatusChangeByUserId,
                            Note = change.Note,
                            ProductName = product.Name,
                            ProductSKU = product.Sku,                            
                            StatusId = change.StatusId,
                            StockQuantityChange = change.StockQuantityChange,
                            WareHouseName = warehouse.Name
                        };

            return query.ToList();
        }

        public void UpdateInventoryChange(InventoryChange inventoryChange)
        {
            _inventoryChange.Update(inventoryChange);

            _eventPublisher.EntityUpdated(inventoryChange);

        }

        public int  GetAllInventoryChangeInProcessForWarehousesCount(int[] warehouseIds)
        {

            var query = from change in _inventoryChange.Table
                        join inventory in _productWarehouseInventory.Table on change.InventoryId equals inventory.Id                  
                        where warehouseIds.Contains(inventory.WarehouseId)
                        && change.StatusId == (int)InventoryChangeStatus.Processing
                        select change;

            return query.Count();
        }

        public int GetUnpaidAmount(IList<int> warehouseIds)
        {
            var query = from purchase in _inventoryPurchase.Table
                        where warehouseIds.Contains(purchase.WarehouseId) && purchase.PaymentId == 0
                        select purchase.PriceInclTax;

            return (int)query.Sum();
        }
    }
}

