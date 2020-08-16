using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Distribution;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Inventory;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class InventoryController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IInventoryModelFactory _distributionModelFactory;
        private readonly IWorkContext _workContext;

        public InventoryController(IPermissionService permissionService,
                                    IInventoryModelFactory distributionModelFactory,
                                    IWorkContext workContext)
        {
            _distributionModelFactory = distributionModelFactory;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult List()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
            //    return AccessDeniedView();

            var searchModel = new InventorySearchModel();
            searchModel.CustomerId = _workContext.CurrentCustomer.Id;

            //prepare model
            var model = _distributionModelFactory.PrepareProductInventorySearchModel(searchModel);

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
            var model = _distributionModelFactory.PrepareProductInventoryListModel(searchModel);

            return Json(model);
        }

    }
}
