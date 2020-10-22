using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components
{
    public class DistributorStatistics: NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly IPermissionService _permissionService;

        public DistributorStatistics(ICommonModelFactory commonModelFactory,
          IPermissionService permissionService)
        {
            _commonModelFactory = commonModelFactory;
            _permissionService = permissionService;
        }
        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageInventories))
            {
                return Content(string.Empty);
            }

            //prepare model
            var model = _commonModelFactory.PrepareDistributorStatisticsModel();

            return View(model);
        }
    }
}
