using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Distribution;
using Nop.Services.Catalog;
using Nop.Services.Distribution;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Inventory;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    public class InventoryModelFactory : IInventoryModelFactory
    {
        private readonly ICustomerWarehouseService _customerWarehouseService;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IShipmentService _shipmentService;
        private readonly IInventoryPurchaseService _inventoryPurchaseService;
        public InventoryModelFactory(ICustomerWarehouseService customerWarehouseService,
                                        IInventoryPurchaseService inventoryPurchaseService,
                                        IProductService productService,
                                        IShippingService shippingService,
                                        IShipmentService shipmentService)
        {
            _customerWarehouseService = customerWarehouseService;
            _inventoryPurchaseService = inventoryPurchaseService;
            _productService = productService;
            _shippingService = shippingService;
            _shipmentService = shipmentService;
        }


        public InventoryListModel PrepareProductInventoryListModel(InventorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var warehouseIds = _customerWarehouseService.GetCustomerWarehouseIds(searchModel.CustomerId);

            var productInventories = _productService.GetAllProductWarehouseInventoryRecords(warehouseIds).ToPagedList(searchModel);
            var productIds = productInventories.Select(x => x.ProductId).Distinct().ToArray();
            var products = _productService.GetProductsByIds(productIds, searchModel.ProductName, searchModel.Sku).ToDictionary(x => x.Id);
            var warehouses = _shippingService.GetWarehousesByIds(warehouseIds).ToDictionary(x => x.Id);

            //remove any product inventories that is not in the product list
            for (int i = productInventories.Count - 1; i >= 0; i--)
            {
                if (!products.ContainsKey(productInventories[i].ProductId))
                    productInventories.RemoveAt(i);
            }

            var model = new InventoryListModel().PrepareToGrid(searchModel, productInventories, () =>
            {
                return productInventories.Select(inventory =>
                    {
                        var inventoryModel = inventory.ToModel<InventoryModel>();
                        if (products.TryGetValue(inventoryModel.ProductId, out var product))
                        {
                            inventoryModel.ProductId = product.Id;
                            inventoryModel.ProductName = product.Name;
                            inventoryModel.Sku = product.Sku;
                        }

                        if (warehouses.TryGetValue(inventoryModel.WarehouseId, out var warehouse))
                        {
                            inventoryModel.WarehouseName = warehouse.Name;
                        }

                        inventoryModel.PlannedQuantity = _shipmentService.GetQuantityInShipments(product, inventoryModel.WarehouseId, true, true);

                        return inventoryModel;
                    }
                );

            });

            return model;

        }

        public virtual ManageInventoryModel PrepareManageInventoryModel(ManageInventoryModel manageInventoryModel)
        {
            if (manageInventoryModel == null)
                throw new ArgumentNullException(nameof(manageInventoryModel));

            manageInventoryModel.MultipleWarehouses = _customerWarehouseService.GetCustomerWarehouseIds(manageInventoryModel.CustomerId).Length > 1;
            manageInventoryModel.InventoryPurchaseSearchModel.MultipleWarehouses = manageInventoryModel.MultipleWarehouses;
            manageInventoryModel.InventorySearchModel.MultipleWarehouses = manageInventoryModel.MultipleWarehouses;
            manageInventoryModel.InventoryPurchaseSearchModel.CustomerId = manageInventoryModel.CustomerId;
            manageInventoryModel.InventorySearchModel.CustomerId = manageInventoryModel.CustomerId;

            return manageInventoryModel;
        }

        public InventoryPurchaseListModel PrepareInventoryPurchaseSearchListModel(InventoryPurchaseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var warehouseIds = _customerWarehouseService.GetCustomerWarehouseIds(searchModel.CustomerId).ToList();
            var warehouseNamesDict = _shippingService.GetWarehousesByIds(warehouseIds.ToArray()).ToDictionary(k => k.Id, v => v.Name);

            if (!String.IsNullOrEmpty(searchModel.WarehouseName))
            {
                foreach (var warehouseName in warehouseNamesDict)
                {
                    if (!warehouseName.Value.Contains(searchModel.WarehouseName, StringComparison.OrdinalIgnoreCase))
                    {
                        warehouseIds.Remove(warehouseName.Key);
                    }
                }
            }

            IPagedList<InventoryPurchase> inventoriesPurchased = null;
            if (searchModel.ApprovalMode && !searchModel.UnapprovedOnly)
            {
                inventoriesPurchased = _inventoryPurchaseService.GetInventoryPurchasesByWarehouseIds(warehouseIds).ToPagedList(searchModel);
            }
            else
                inventoriesPurchased = _inventoryPurchaseService.GetInventoryPurchases(InventoryPurchaseServiceDefaults.Status.Pending, warehouseIds).ToPagedList(searchModel);

            var model = new InventoryPurchaseListModel().PrepareToGrid(searchModel, inventoriesPurchased, () =>
            {
                return inventoriesPurchased.Select(inventory =>
                {
                    var inventoryModel = inventory.ToModel<InventoryPurchaseModel>();
                    var product = _productService.GetProductById(inventory.ProductId);
                    inventoryModel.ProductName = product.Name;
                    inventoryModel.Sku = product.Sku;
                    if (warehouseNamesDict.TryGetValue(inventoryModel.WarehouseId, out var warehouseName))
                        inventoryModel.WarehouseName = warehouseName;
                    return inventoryModel;
                }
                );
            });

            return model;
        }

    }
}
