using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Distribution;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Inventory;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Nop.Core.Domain.Distribution;
using Nop.Services.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Messages;
using System.Net;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class InventoryController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IInventoryModelFactory _inventoryModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IInventoryPurchaseService _inventoryPurchaseService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;

        public InventoryController(IPermissionService permissionService,
                                    IInventoryModelFactory distributionModelFactory,
                                    IInventoryPurchaseService inventoryPurchaseService,
                                    IWorkContext workContext,
                                    IGenericAttributeService genericAttributeService,
                                    IProductService productService,
                                    IStoreContext storeContext,
                                    INotificationService notificationService)
        {
            _inventoryModelFactory = distributionModelFactory;
            _inventoryPurchaseService = inventoryPurchaseService;
            _permissionService = permissionService;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
            _productService = productService;
            _storeContext = storeContext;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult List()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
            //    return AccessDeniedView();

            var manageInventoryModel = new ManageInventoryModel();
            manageInventoryModel.CustomerId = _workContext.CurrentCustomer.Id;

            manageInventoryModel.Amount = _genericAttributeService.GetAttribute<int>(_workContext.CurrentCustomer, InventoryPurchaseServiceDefaults.PurchaseRecordDefaultAmount);
            if (manageInventoryModel.Amount == 0)
                manageInventoryModel.Amount = 1;

            //prepare model
            var model = _inventoryModelFactory.PrepareManageInventoryModel(manageInventoryModel);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(InventorySearchModel searchModel)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
            //    return AccessDeniedDataTablesJson();

            if(searchModel.CustomerId == 0)
            searchModel.CustomerId = _workContext.CurrentCustomer.Id;

            //prepare model
            var model = _inventoryModelFactory.PrepareProductInventoryListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult GetUnfinalisedPurchaseRecord()
        {
            var searchModel = new InventoryPurchaseSearchModel();
            searchModel.CustomerId = _workContext.CurrentCustomer.Id;
            searchModel.ApprovalMode = false;

            var model = _inventoryModelFactory.PrepareInventoryPurchaseSearchListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GetUnfinalisedPurchaseRecord(InventoryPurchaseSearchModel searchModel)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
            //    return AccessDeniedDataTablesJson();

            if (searchModel.CustomerId == 0)
                searchModel.CustomerId = _workContext.CurrentCustomer.Id;

            //prepare model
            var model = _inventoryModelFactory.PrepareInventoryPurchaseSearchListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult GetPurchaseRecordForApproval()
        {
            var searchModel = new InventoryPurchaseSearchModel();
            searchModel.CustomerId = _workContext.CurrentCustomer.Id;
            searchModel.ApprovalMode = true;

            var model = _inventoryModelFactory.PrepareInventoryPurchaseSearchListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GetPurchaseRecordForApproval(InventoryPurchaseSearchModel searchModel)
        {
            searchModel.ApprovalMode = true;
            searchModel.CustomerId = _workContext.CurrentCustomer.Id;
            var model = _inventoryModelFactory.PrepareInventoryPurchaseSearchListModel(searchModel);

            return Json(model);
        }


        [HttpPost]
        public virtual IActionResult DeletePurchaseRecord(int id)
        {
            var invPurchase = _inventoryPurchaseService.GetInventoryPurchase(id);

            if(invPurchase!=null)
            {
                _inventoryPurchaseService.DeleteInventoryPurchase(id);
                var pwi = _productService.GeProductWarehouseInventoryRecord(invPurchase.ProductId, invPurchase.WarehouseId);
                pwi.StockQuantity += invPurchase.Quantity;
                _productService.UpdateProductWarehouseInventory(pwi);
            }                        

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult PurchaseInventories(ICollection<InventoryPurchaseModel> selectedInventories, int quantity)
        {
             _genericAttributeService.SaveAttribute<int>(_workContext.CurrentCustomer, InventoryPurchaseServiceDefaults.PurchaseRecordDefaultAmount, quantity);
            
            var inventoryPurchases = new List<InventoryPurchase>();
            var productWarehouseUpdates = new List<ProductWarehouseInventory>();            

            foreach (var inventory in selectedInventories)
            {
                var product = _productService.GetProductById(inventory.ProductId);
                var pwi = _productService.GeProductWarehouseInventoryRecord(product.Id, inventory.WarehouseId);

                if (pwi.StockQuantity < quantity)
                {
                    return Json(new { result = false, message = $"Error: Pembelian melebihi stok untuk produk {product.Name}." });
                }

                pwi.StockQuantity -= quantity;
                productWarehouseUpdates.Add(pwi);

                var inventoryPurchase = new InventoryPurchase();
                inventoryPurchase.ProductId = product.Id;
                inventoryPurchase.WarehouseId = inventory.WarehouseId;
                inventoryPurchase.Quantity = quantity;
                inventoryPurchase.Status = InventoryPurchaseServiceDefaults.Status.Pending;
                inventoryPurchase.PurchasedDate = DateTime.Now;
                inventoryPurchase.PriceInclTax = product.Price;
                if (product.HasTierPrices)
                    inventoryPurchase.PriceInclTax = _productService.GetPreferredTierPrice(product, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id, quantity).Price;
                inventoryPurchases.Add(inventoryPurchase);                                                
            }

             _inventoryPurchaseService.AddInventoryPurchase(inventoryPurchases);
             _productService.UpdateProductWarehouseInventory(productWarehouseUpdates);
             
            return Json(new { result = true });
        }

        public virtual IActionResult ListInventoryApproval()
        {
            var manageInventoryModel = new ManageInventoryModel();
            manageInventoryModel.CustomerId = _workContext.CurrentCustomer.Id;
            manageInventoryModel.InventoryPurchaseSearchModel.ApprovalMode = true;

            //prepare model
            var model = _inventoryModelFactory.PrepareManageInventoryModel(manageInventoryModel);           

            return View(model);
        }

        public virtual IActionResult ApproveSelectedPurchase(int id)
        {
            //if (!_permissionService.Authorize()
            //    return AccessDeniedView();
            var purchase = _inventoryPurchaseService.GetInventoryPurchase(id);
            if(purchase!=null)
            {
                purchase.Status = InventoryPurchaseServiceDefaults.Status.Closed;
                _inventoryPurchaseService.UpdateInventoryPurchase(purchase);
            }

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual IActionResult ApproveSelectedPurchases(int[] selectedIds)
        {
            //if (!_permissionService.Authorize()
            //    return AccessDeniedView();
            var purchases = _inventoryPurchaseService.GetInventoryPurchasesByIds(selectedIds);
            if (purchases.Count > 0 )
            {
                foreach (var item in purchases)
                {
                    item.Status = InventoryPurchaseServiceDefaults.Status.Closed;
                }
               
                _inventoryPurchaseService.UpdateInventoryPurchases(purchases);
            }

            return Json(new { result = true });
        }
    }
}
