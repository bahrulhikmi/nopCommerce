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
using Nop.Services.Helpers;
using Nop.Services.Customers;
using Nop.Services.Localization;

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
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILocalizationService _localizationService;

        public InventoryController(IPermissionService permissionService,
                                    IInventoryModelFactory distributionModelFactory,
                                    IInventoryPurchaseService inventoryPurchaseService,
                                    IWorkContext workContext,
                                    IGenericAttributeService genericAttributeService,
                                    IProductService productService,
                                    IStoreContext storeContext,
                                    INotificationService notificationService,
                                    IPriceFormatter priceFormatter,
                                    ILocalizationService localizationService
                                    )
        {
            _inventoryModelFactory = distributionModelFactory;
            _inventoryPurchaseService = inventoryPurchaseService;
            _permissionService = permissionService;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
            _productService = productService;
            _storeContext = storeContext;
            _notificationService = notificationService;
            _priceFormatter = priceFormatter;
            _localizationService = localizationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult List()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
            //    return AccessDeniedView();

            var manageInventoryModel = new ManageInventoryPurchasePaymentModel();
            manageInventoryModel.CustomerId = _workContext.CurrentCustomer.Id;

            manageInventoryModel.InventorySearchModel.Amount = _genericAttributeService.GetAttribute<int>(_workContext.CurrentCustomer, InventoryPurchaseServiceDefaults.PurchaseRecordDefaultAmount);
            if (manageInventoryModel.InventorySearchModel.Amount == 0)
                manageInventoryModel.InventorySearchModel.Amount = 1;

            //prepare model
            var model = _inventoryModelFactory.PrepareManageInventoryModel(manageInventoryModel);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(InventorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            if (searchModel.CustomerId == 0)
                searchModel.CustomerId = _workContext.CurrentCustomer.Id;

            //prepare model
            var model = _inventoryModelFactory.PrepareProductInventoryListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult GetUnfinalisedPurchaseRecord()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            var searchModel = new InventoryPurchaseSearchModel();

            searchModel.CustomerId = _workContext.CurrentCustomer.Id;

            var model = _inventoryModelFactory.PrepareInventoryPurchaseSearchListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GetNotInPaymentPurchaseRecord(InventoryPurchaseSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            //prepare model
            var model = _inventoryModelFactory.PrepareNotInPaymentInventoryPurchaseSearchListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GetUnfinalisedPurchaseRecord(InventoryPurchaseSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedDataTablesJson();

            if (searchModel.CustomerId == 0)
                searchModel.CustomerId = _workContext.CurrentCustomer.Id;

            //prepare model
            var model = _inventoryModelFactory.PrepareInventoryPurchaseSearchListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GetPurchaseRecordIncludedInPayment(InventoryPurchaseSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _inventoryModelFactory.PrepareIncludedInPaymentInventoryPurchaseSearchListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult GetPurchasePaymentRecords()
        {
            var searchModel = new InventoryPurchasePaymentSearchModel();

            var model = _inventoryModelFactory.PrepareInventoryPurchasePaymentSearchListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GetPurchasePaymentRecords(InventoryPurchasePaymentSearchModel searchModel)
        {
            var model = _inventoryModelFactory.PrepareInventoryPurchasePaymentSearchListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GetMyPurchasePaymentRecords(InventoryPurchasePaymentSearchModel searchModel)
        {
            searchModel.MyPaymentOnly = true;
            var model = _inventoryModelFactory.PrepareInventoryPurchasePaymentSearchListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GetPaymentTotal(int id)
        {
            var inventoryPurchasePayment = _inventoryPurchaseService.GetInventoryPurchasePayment(id);
            return Json(new { result = true, total = _priceFormatter.FormatPrice(inventoryPurchasePayment.Total) });
        }

        [HttpPost]
        public virtual IActionResult DeletePurchaseRecord(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            var invPurchase = _inventoryPurchaseService.GetInventoryPurchase(id);

            if (invPurchase != null && !invPurchase.AdditionalFee)
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

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
                inventoryPurchase.PurchasedDateUtc = DateTime.Now;
                var price = product.Price;
                if (product.HasTierPrices)
                    price = _productService.GetPreferredTierPrice(product, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id, quantity).Price;

                price *= quantity;
                inventoryPurchase.PriceInclTax = price;
                inventoryPurchases.Add(inventoryPurchase);
            }

            _inventoryPurchaseService.AddInventoryPurchase(inventoryPurchases);
            _productService.UpdateProductWarehouseInventory(productWarehouseUpdates);

            return Json(new { result = true });
        }

        public virtual IActionResult ListInventoryPurchasePayment()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventoriesApproval))
                return AccessDeniedView();

            var manageInventoryModel = new InventoryPurchasePaymentSearchModel();

            return View(manageInventoryModel);
        }

        [HttpPost]
        public virtual IActionResult ApproveSelectedPurchase(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventoriesApproval))
                return AccessDeniedView();

            var purchasePayment = _inventoryPurchaseService.GetInventoryPurchasePayment(id);
            if (purchasePayment != null)
            {
                purchasePayment.Status = InventoryStatus.Complete;
                _inventoryPurchaseService.UpdateInventoryPurchasePayment(purchasePayment);
            }

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual IActionResult ApproveSelectedPurchases(int[] selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventoriesApproval))
                return AccessDeniedView();

            var purchasePayment = _inventoryPurchaseService.GetInventoryPurchasePayments(selectedIds);
            foreach (var item in purchasePayment)
            {
                item.Status = InventoryStatus.Complete;
            }

            _inventoryPurchaseService.UpdateInventoryPurchasePayments(purchasePayment);
            return Json(new { result = true });
        }

        [HttpPost]
        public virtual IActionResult Pay(int[] selectedPaymentIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            var pendingPaymentPurchase = _inventoryPurchaseService.
                CreateOrGetExistingPendingPurchasePayment(new InventoryPurchasePayment
                {
                    CustomerId = _workContext.CurrentCustomer.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });

            _inventoryPurchaseService.AddInventoryPurchasePayment(pendingPaymentPurchase, selectedPaymentIds);

            return Json(new { result = true, id = pendingPaymentPurchase.Id });
        }

        public virtual IActionResult EditPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            var model = new InventoryPurchasePaymentModel();
            var inventoryPurchasePayment = _inventoryPurchaseService.GetInventoryPurchasePayment(id);

            if (inventoryPurchasePayment == null)
            {
                model.ReadOnly = true;
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Common.Alert.Read.Error"));
                return View(model);
            }

            if (_workContext.CurrentCustomer.Id != inventoryPurchasePayment.CustomerId)
                return AccessDeniedView();

            model = _inventoryModelFactory.PrepareUpdateInventoryPurchasePaymentModel(inventoryPurchasePayment);

            return View(model);
        }

        public virtual IActionResult ViewPaymentForApproval(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            var inventoryPurchasePayment = _inventoryPurchaseService.GetInventoryPurchasePayment(id);

            var model = _inventoryModelFactory.PrepareUpdateInventoryPurchasePaymentModel(inventoryPurchasePayment);
            model.AllowApproval = true;

            return View(model);
        }


        [HttpPost]
        public virtual IActionResult ConfirmPayment(InventoryPurchasePayment model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (model.DownloadId == 0)
                {
                    return Json(new { result = false, message = _localizationService.GetResource("Admin.Inventories.Payment.TransferReceiptIsMandatory") });
                }
                else
                {
                    var inventoryPurchasePayment = _inventoryPurchaseService.GetInventoryPurchasePayment(model.Id);

                    if (_workContext.CurrentCustomer.Id != inventoryPurchasePayment.CustomerId)
                        return AccessDeniedView();


                    inventoryPurchasePayment.DownloadId = model.DownloadId;
                    inventoryPurchasePayment.Note = model.Note;
                    inventoryPurchasePayment.Status = InventoryStatus.Processing;

                    _inventoryPurchaseService.UpdateInventoryPurchasePayment(inventoryPurchasePayment);

                    _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Inventories.Payment.Confirmed"));

                }
            }

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual IActionResult CancelPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            var inventoryPurchasePayment = _inventoryPurchaseService.GetInventoryPurchasePayment(id);

            if (_workContext.CurrentCustomer.Id != inventoryPurchasePayment.CustomerId)
                return AccessDeniedView();

            _inventoryPurchaseService.DeleteInventoryPurchasePayment(inventoryPurchasePayment);

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual IActionResult ExcludeFromPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            var inventoryPurchase = _inventoryPurchaseService.GetInventoryPurchase(id);
            var inventoryPurchasePayment = _inventoryPurchaseService.GetInventoryPurchasePayment(inventoryPurchase.PaymentId);

            if (inventoryPurchasePayment == null)
                return Json(new { result = false });

            inventoryPurchase.PaymentId = 0;
            inventoryPurchasePayment.Total -= inventoryPurchase.PriceInclTax;

            _inventoryPurchaseService.UpdateInventoryPurchase(inventoryPurchase);
            _inventoryPurchaseService.UpdateInventoryPurchasePayment(inventoryPurchasePayment);

            return Json(new { result = true, total = inventoryPurchasePayment.Total });
        }

        [HttpPost]
        public virtual IActionResult IncludeSelectedPurchase(int id, int paymentId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            var inventoryPurchase = _inventoryPurchaseService.GetInventoryPurchase(id);
            var inventoryPurchasePayment = _inventoryPurchaseService.GetInventoryPurchasePayment(paymentId);

            if (inventoryPurchasePayment == null || inventoryPurchase.PaymentId > 0)
                return Json(new { result = false });

            inventoryPurchase.PaymentId = paymentId;
            inventoryPurchasePayment.Total += inventoryPurchase.PriceInclTax;

            _inventoryPurchaseService.UpdateInventoryPurchase(inventoryPurchase);
            _inventoryPurchaseService.UpdateInventoryPurchasePayment(inventoryPurchasePayment);

            return Json(new { result = true, total = inventoryPurchasePayment.Total });
        }

        public virtual IActionResult AddInventory(int id)
        {
            var inventoryModel = new AddInventoryModel();
            inventoryModel.PreSelectedWarehouseId = id;
            var model = _inventoryModelFactory.PrepareAddInventoryModel(inventoryModel);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ApproveInventoryChange(AddInventoryModel inventoryModel)
        {


            var inventoryItems = _productService.GetProductWarehouseInventoryRecords(
                inventoryModel.InventoryAdditions.Select(x => x.Id).ToArray());

            var amountDict = inventoryModel.InventoryAdditions.ToDictionary(key => key.Id, val => val.Amount);
            foreach (var item in inventoryItems)
            {
                item.StockQuantity += amountDict[item.Id];
            }

            _productService.UpdateProductWarehouseInventory(inventoryItems);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Inventories.Inventories.Updated"));

            return RedirectToAction("AddInventory");
        }

        [HttpPost]
        public virtual IActionResult AddInventory(AddInventoryModel inventoryModel)
        {

            var inventoryPurchase = new InventoryPurchase
            {
                WarehouseId = inventoryModel.WarehouseId,
                PriceInclTax = inventoryModel.AdditionalFee,
                Note = inventoryModel.Note,
                PurchasedDateUtc = DateTime.UtcNow,
                AdditionalFee = true,
                Quantity = 1
            };

            var inventoryItems = _productService.GetProductWarehouseInventoryRecords(
                inventoryModel.InventoryAdditions.Select(x => x.Id).ToArray());

            var amountDict = inventoryModel.InventoryAdditions.ToDictionary(key => key.Id, val => val.Amount);
            var listChangeInventory = new List<InventoryChange>();
            foreach (var item in inventoryItems)
            {
                var inventoryChange = new InventoryChange();
                inventoryChange.Description = "Admin.Inventory.InventoryChanges.Added";
                inventoryChange.CreatedByUserId = _workContext.CurrentCustomer.Id;
                inventoryChange.Status = InventoryChangeStatus.Processing;
                inventoryChange.StockQuantityChange = amountDict[item.Id];
                inventoryChange.InventoryId = item.Id;
                inventoryChange.DateUtc = DateTime.UtcNow;
                listChangeInventory.Add(inventoryChange);
            }

            _inventoryPurchaseService.AddInventoryPurchase(inventoryPurchase);
            _inventoryPurchaseService.AddInventoryChanges(listChangeInventory);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Inventories.Inventories.Updated"));

            return RedirectToAction("AddInventory");
        }

        public IActionResult AllInventories()
        {
            var model = new AllInventoryModel();

            _inventoryModelFactory.PrepareAllInventoryModel(model);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListAll(InventorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageInventoriesApproval))
                return AccessDeniedView();

            if (searchModel.CustomerId == 0)
                searchModel.CustomerId = _workContext.CurrentCustomer.Id;

            //prepare model
            var model = _inventoryModelFactory.PrepareProductInventoryListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GetInventoryChangesInProcess(InventoryChangeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories) ||
                !_permissionService.Authorize(StandardPermissionProvider.ManageInventoriesApproval))
                return AccessDeniedView();

            //prepare model
            var model = _inventoryModelFactory.PrepareInventoryChangesInProcessListModel(searchModel);

            return Json(model);
        }
        public virtual IActionResult MyInventoryChanges()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            var searchModel = new MyInventoryChangesModel();
            var model = _inventoryModelFactory.PrepareMyInventoryChangesModel(searchModel);

            return View(model);
        }


        [HttpPost]
        public virtual IActionResult GetInventoryChangesInProcessForApproval(InventoryChangeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();


            searchModel.ForUserId = _workContext.CurrentCustomer.Id;

            //prepare model
            var model = _inventoryModelFactory.PrepareInventoryChangesInProcessListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GetInventoryChangesHistory(InventoryChangeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();

            searchModel.ForUserId = _workContext.CurrentCustomer.Id;
            //prepare model
            var model = _inventoryModelFactory.PrepareInventoryChangesHistoryModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ApproveSelectedInventoryChange(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
                return AccessDeniedView();


            var inventoryChange = _inventoryPurchaseService.GetInventoryChange(id);

            if(inventoryChange.Status != InventoryChangeStatus.Processing)
            {
                return Json(new { result = false, message = $"Error: Inventory Status tidak valid." });
            }

            if (inventoryChange == null)
            {
                return Json(new { result = false, message = $"Error: Perubahan Inventory tidak ditemukan." });
            }
            //prepare model

            var inventory = _productService.GetProductWarehouseInventoryRecords(new int[] { inventoryChange.InventoryId }).FirstOrDefault();

            if (inventory == null)
            {
                return Json(new { result = false, message = $"Error: Inventory tidak ditemukan." });
            }
            //p
            
            inventory.StockQuantity += inventoryChange.StockQuantityChange;
            inventoryChange.Status = InventoryChangeStatus.Complete;
            inventoryChange.LastStatusChangeByUserId = _workContext.CurrentCustomer.Id;            

            _productService.UpdateProductWarehouseInventory(inventory);
            _inventoryPurchaseService.UpdateInventoryChange(inventoryChange);

            return Json(new { result = true});
        }

    }
}
