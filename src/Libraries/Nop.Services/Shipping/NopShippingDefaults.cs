using Nop.Core.Caching;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Represents default values related to shipping services
    /// </summary>
    public static partial class NopShippingDefaults
    {
        #region Caching defaults

        #region Shipping methods

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : country identifier
        /// </remarks>
        public static CacheKey ShippingMethodsAllCacheKey => new CacheKey("Nop.shippingmethod.all-{0}", ShippingMethodsAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ShippingMethodsAllPrefixCacheKey => "Nop.shippingmethod.all";

        #endregion

        #region Warehouses

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static CacheKey WarehousesAllCacheKey => new CacheKey("Nop.warehouse.all");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// {1} : address identifier
        /// </remarks>
        public static CacheKey WarehousesByIdsCacheKey => new CacheKey("Nop.warehouses.by.ids-{0}", WarehousesPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string WarehousesPrefixCacheKey => "Nop.warehouse";

        #endregion

        #region Date

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static CacheKey DeliveryDatesAllCacheKey => new CacheKey("Nop.deliverydates.all");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static CacheKey ProductAvailabilityAllCacheKey => new CacheKey("Nop.productavailability.all");

        #endregion

        #endregion
    }
}