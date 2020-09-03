using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Distribution;
using Nop.Services.Catalog;
using Nop.Data;
using System.Linq;
using Nop.Services.Events;
using Nop.Core;

namespace Nop.Services.Distribution
{
    public class InventoryPurchaseService : IInventoryPurchaseService  
    {

        private readonly IRepository<InventoryPurchase> _inventoryPurchase;      
        private readonly IEventPublisher _eventPublisher;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
       
        public InventoryPurchaseService(IRepository<InventoryPurchase> inventoryPurchase,
                                        IEventPublisher eventPublisher,
                                        IProductService productService, 
                                        IWorkContext workContext,
                                        IStoreContext storeContext)
        {
            _eventPublisher = eventPublisher;
            _inventoryPurchase = inventoryPurchase;
            _productService = productService;
            _workContext = workContext;
            _storeContext = storeContext;
        }
        public IList<InventoryPurchase> GetInventoryPurchases(string status, IList<int> warehouseIds)
        {
            var query = from purchase in _inventoryPurchase.Table
                        where purchase.Status == status && warehouseIds.Contains(purchase.WarehouseId)
                        select purchase;

            return query.ToList();
        }

        public IList<InventoryPurchase> GetInventoryPurchasesByWarehouseIds(IList<int> warehouseIds)
        {
            var query = from purchase in _inventoryPurchase.Table
                        where warehouseIds.Contains(purchase.WarehouseId)
                        select purchase;

            return query.ToList();
        }

        public IList<InventoryPurchase> GetInventoryPurchasesByIds(IList<int> warehouseIds)
        {
            var query = from purchase in _inventoryPurchase.Table
                        where warehouseIds.Contains(purchase.Id)
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
    }
}

