using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Distribution;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Distribution;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Inventory;
using Nop.Web.Framework.Models.Extensions;
using Nop.Services.Localization;

namespace Nop.Web.Areas.Admin.Factories
{
    public class InventoryModelFactory : IInventoryModelFactory
    {
        private readonly ICustomerWarehouseService _customerWarehouseService;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IShipmentService _shipmentService;
        private readonly IInventoryPurchaseService _inventoryPurchaseService;
        private readonly IDownloadService _downloadService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;

        public InventoryModelFactory(ICustomerWarehouseService customerWarehouseService,
                                        IInventoryPurchaseService inventoryPurchaseService,
                                        IProductService productService,
                                        IShippingService shippingService,
                                        IShipmentService shipmentService,
                                        IDownloadService downloadService,
                                        IWorkContext workContext,
                                        IDateTimeHelper dateTimeHelper,
                                        ICustomerService customerService,
                                        ILocalizationService localizationService)
        {
            _customerWarehouseService = customerWarehouseService;
            _inventoryPurchaseService = inventoryPurchaseService;
            _productService = productService;
            _shippingService = shippingService;
            _shipmentService = shipmentService;
            _downloadService = downloadService;
            _workContext = workContext;
            _dateTimeHelper = dateTimeHelper;
            _customerService = customerService;
            _localizationService = localizationService;
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

        public InventoryPurchasePaymentListModel PrepareInventoryPurchasePaymentSearchListModel(InventoryPurchasePaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var warehouseIds = _customerWarehouseService.GetCustomerWarehouseIds(_workContext.CurrentCustomer.Id).ToList();
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

            IPagedList<InventoryPurchasePayment> inventoriesPurchasePayment = null;

            if(!searchModel.MyPaymentOnly)
                inventoriesPurchasePayment =  _inventoryPurchaseService.GetUnapprovedInventoryPurchasePayments().ToPagedList(searchModel);
            else
                inventoriesPurchasePayment = _inventoryPurchaseService.GetIncompleteInventoryPurchasePaymentsForCustomer(_workContext.CurrentCustomer.Id).ToPagedList(searchModel);

            var model = new InventoryPurchasePaymentListModel().PrepareToGrid(searchModel, inventoriesPurchasePayment, () =>
            {
                return inventoriesPurchasePayment.Select(inventory =>
                {
                    var inventoryModel = inventory.ToModel<InventoryPurchasePaymentModel>();
                    inventoryModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(inventory.CreatedOnUtc, DateTimeKind.Utc);
                    var cust = _customerService.GetCustomerById(inventory.CustomerId);                   
                    inventoryModel.User = _customerService.GetCustomerFullName(_workContext.CurrentCustomer);
                    if (inventoryModel.DownloadId > 0)
                        inventoryModel.DownloadGuid = _downloadService.GetDownloadById(inventoryModel.DownloadId).DownloadGuid;
                    return inventoryModel;
                }
                );
            });

            return model;
        }

        public InventoryPurchasePaymentModel PrepareUpdateInventoryPurchasePaymentModel(InventoryPurchasePayment inventoryPurchasePayment)
        {
            var model = new InventoryPurchasePaymentModel();

            model.AllowApproval = false;
            model = inventoryPurchasePayment.ToModel<InventoryPurchasePaymentModel>();
            model.AvailableInventoryPurchaseSearchModel.PaymentId = inventoryPurchasePayment.Id;
            model.PaymentId = inventoryPurchasePayment.Id;
            var inventories = _inventoryPurchaseService.GetInventoryPurchasesByPaymentId(inventoryPurchasePayment.Id).
                Select(x => x.ToModel<InventoryPurchaseModel>()).ToList();

            foreach (var item in inventories)
            {
                var product = _productService.GetProductById(item.ProductId);
                item.ProductName = product.Name;
                item.Sku = product.Sku;
            }

            model.DownloadGuid = _downloadService.GetDownloadById(model.DownloadId)?.DownloadGuid ?? Guid.Empty;
            model.ReadOnly = inventoryPurchasePayment.Status == InventoryStatus.Processing;
            model.AddedInventoryPurchaseSearchModel = inventories;
            model.IncludedInventoryPurchaseSearchModel.PaymentId = inventoryPurchasePayment.Id;

            return model;
        }

        public InventoryPurchaseListModel PrepareInventoryPurchaseSearchListModel(InventoryPurchaseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var warehouseIds = _customerWarehouseService.GetCustomerWarehouseIds(_workContext.CurrentCustomer.Id).ToList();
            var warehouseNamesDict = _shippingService.GetWarehousesByIds(warehouseIds.ToArray()).ToDictionary(k => k.Id, v => v.Name);

            //Remove any warehouses that doesn't match the search criteria
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

            IPagedList<InventoryPurchase> inventoriesPurchased = _inventoryPurchaseService.GetInventoryPurchasesWithoutPaymentId(warehouseIds).ToPagedList(searchModel);
       
            //Get Payment status
            var paymentIds = inventoriesPurchased.Select(x => x.PaymentId).Distinct();

            var purchasePayments = new Dictionary<int, InventoryPurchasePayment>();
            if (paymentIds.Count() > 0)
            {
                purchasePayments = _inventoryPurchaseService.GetInventoryPurchasePayments(paymentIds.ToArray()).ToDictionary(x => x.Id, y => y);
            }

            var model = new InventoryPurchaseListModel().PrepareToGrid(searchModel, inventoriesPurchased, () =>
            {
                return inventoriesPurchased.Where(x => (x.PaymentId == 0) || purchasePayments[x.PaymentId].StatusId <= ((int)InventoryStatus.Processing))
                .Select(inventory =>
                {
                    var inventoryModel = inventory.ToModel<InventoryPurchaseModel>();
                    var product = _productService.GetProductById(inventory.ProductId);
                    inventoryModel.ProductName = product.Name;
                    inventoryModel.Sku = product.Sku;
                    if (inventoryModel.PaymentId > 0)
                    {
                        inventoryModel.StatusId = purchasePayments[inventoryModel.PaymentId].StatusId;
                        switch (purchasePayments[inventoryModel.PaymentId].Status)
                        {
                            case InventoryStatus.Cancelled:
                                inventoryModel.Status = _localizationService.GetResource("Admin.Common.PaymentStatus.Cancelled");
                                break;
                            case InventoryStatus.Complete:
                                inventoryModel.Status = _localizationService.GetResource("Admin.Common.PaymentStatus.Complete");
                                break;
                            case InventoryStatus.Pending:
                                inventoryModel.Status = _localizationService.GetResource("Admin.Common.PaymentStatus.Pending");
                                break;
                            case InventoryStatus.Processing:
                                inventoryModel.Status = _localizationService.GetResource("Admin.Common.PaymentStatus.Processing");
                                break;
                        }
                    }
                    else
                        inventoryModel.Status = _localizationService.GetResource("Admin.Common.PaymentStatus.None");

                    if (warehouseNamesDict.TryGetValue(inventoryModel.WarehouseId, out var warehouseName))
                        inventoryModel.WarehouseName = warehouseName;
                    return inventoryModel;
                }
                );
            });

            return model;
        }

        public InventoryPurchaseListModel PrepareIncludedInPaymentInventoryPurchaseSearchListModel(InventoryPurchaseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));   
            
            
            IPagedList<InventoryPurchase> inventoriesPurchased = _inventoryPurchaseService.GetInventoryPurchasesByPaymentId(searchModel.PaymentId).ToPagedList(searchModel);

            var model = new InventoryPurchaseListModel().PrepareToGrid(searchModel, inventoriesPurchased, () =>
            {
                return inventoriesPurchased.Select(inventory =>
                {
                    var inventoryModel = inventory.ToModel<InventoryPurchaseModel>();
                    var product = _productService.GetProductById(inventory.ProductId);
                    inventoryModel.ProductName = product.Name;
                    inventoryModel.Sku = product.Sku;      
                    return inventoryModel;
                }
                );
            });

            return model;
        }

        public InventoryPurchaseListModel PrepareNotInPaymentInventoryPurchaseSearchListModel(InventoryPurchaseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var warehouseIds = _customerWarehouseService.GetCustomerWarehouseIds(_workContext.CurrentCustomer.Id).ToList();
            var warehouseNamesDict = _shippingService.GetWarehousesByIds(warehouseIds.ToArray()).ToDictionary(k => k.Id, v => v.Name);

            //Remove any warehouses that doesn't match the search criteria
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

            IPagedList<InventoryPurchase> inventoriesPurchased =
                inventoriesPurchased = _inventoryPurchaseService.GetInventoryPurchasesWithoutPaymentId(warehouseIds).ToPagedList(searchModel);

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