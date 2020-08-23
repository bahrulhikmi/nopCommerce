using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Distribution;
using Nop.Data;
using System.Linq;
using Nop.Services.Events;

namespace Nop.Services.Distribution
{
    class InventoryPurchaseService : IInventoryPurchaseService  
    {

        private readonly IRepository<InventoryPurchase> _inventoryPurchase;
        private readonly IEventPublisher _eventPublisher;
        public InventoryPurchaseService(IRepository<InventoryPurchase> inventoryPurchase,
                                        IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
            _inventoryPurchase = inventoryPurchase;           
        }
        public IList<InventoryPurchase> GetInventoryPurchases(string status)
        {
            var query = from purchase in _inventoryPurchase.Table
                        where purchase.Status == status
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

            _inventoryPurchase.Insert(inventoryPurchases);

            //event notification
            _eventPublisher.EntityInserted(inventoryPurchases[0]);
        }
    }
}
