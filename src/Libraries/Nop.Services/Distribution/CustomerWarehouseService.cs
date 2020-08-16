using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Distribution;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Services.Distribution
{
    public class CustomerWarehouseService : ICustomerWarehouseService
    {
        #region Fields
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<CustomerWarehouseMapping> _customerWarehouseMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        #endregion

        #region ctor
        public CustomerWarehouseService(
            ICacheKeyService cacheKeyService,
            IEventPublisher eventPublisher,
            IRepository<Customer> customerRepository,
            IRepository<CustomerWarehouseMapping> customerWarehouseMappingRepository,
            IRepository<Warehouse> warehouseRepository,
            IStaticCacheManager staticCacheManager)
        {
            _cacheKeyService = cacheKeyService;
            _eventPublisher = eventPublisher;
            _customerRepository = customerRepository;
            _customerWarehouseMappingRepository = customerWarehouseMappingRepository;
            _warehouseRepository = warehouseRepository;
            _staticCacheManager = staticCacheManager;
        }
        #endregion

        public void AddCustomerWarehouseMapping(int customerId, int warehouseId)
        {
            if (_customerWarehouseMappingRepository.Table.FirstOrDefault(m => m.CustomerId == customerId && m.WarehouseId == warehouseId) is null)
            {
                var mapping = new CustomerWarehouseMapping
                {
                    WarehouseId= warehouseId,
                    CustomerId = customerId
                };

                _customerWarehouseMappingRepository.Insert(mapping);

                //event notification
                _eventPublisher.EntityInserted(mapping);
            }
        }

        public void DeleteCustomerWarehouseMapping(int customerId, int warehouseId)
        {
            var mapping = _customerWarehouseMappingRepository.Table.SingleOrDefault(cwm => cwm.CustomerId == customerId && cwm.WarehouseId == warehouseId);

            if (mapping != null)
            {
                _customerWarehouseMappingRepository.Delete(mapping);

                //event notification
                _eventPublisher.EntityDeleted(mapping);
            }
        }

        public IList<Customer> GetCustomersManagingWarehouse(int warehouseId)
        {
            var query = from customer in _customerRepository.Table
                        join cam in _customerWarehouseMappingRepository.Table on customer.Id equals cam.CustomerId
                        where cam.WarehouseId == warehouseId
                        select customer;


           return query.ToList();
        }

        public IList<Warehouse> GetWarehousesMangedByCustomer(int customerId)
        {
            var query = from warehouse in _warehouseRepository.Table
                        join cam in _customerWarehouseMappingRepository.Table on warehouse.Id equals cam.WarehouseId
                        where cam.CustomerId == customerId
                        select warehouse;


            return query.ToList();
        }

        /// <summary>
        /// Get customer warehouse identifiers
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <returns>Customer warehouse identifiers</returns>
        public virtual int[] GetCustomerWarehouseIds(int customerId)
        {
            var query = from warehouse in _warehouseRepository.Table
                        join cam in _customerWarehouseMappingRepository.Table on warehouse.Id equals cam.WarehouseId
                        where cam.CustomerId == customerId
                        select warehouse.Id;


           return query.ToArray();
        }

        public void DeleteCustomerWarehouseMappings(int customerId)
        {
           
        }
    }
}
