using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Caching;

namespace Nop.Services.Distribution
{
    class CustomerWarehouseServiceDefaults
    {
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CustomerWarehousePrefixCacheKey => "Nop.Customerwarehouse.";

    }
}
