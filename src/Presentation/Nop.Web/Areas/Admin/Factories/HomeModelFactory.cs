using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Caching;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Home;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the home models factory implementation
    /// </summary>
    public partial class HomeModelFactory : IHomeModelFactory
    {
        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly ILogger _logger;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;
        private readonly NopHttpClient _nopHttpClient;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public HomeModelFactory(AdminAreaSettings adminAreaSettings,
            ICacheKeyService cacheKeyService,
            ICommonModelFactory commonModelFactory,
            ILogger logger,
            IOrderModelFactory orderModelFactory,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext,
            NopHttpClient nopHttpClient,
            IGenericAttributeService genericAttributeService)
        {
            _adminAreaSettings = adminAreaSettings;
            _cacheKeyService = cacheKeyService;
            _commonModelFactory = commonModelFactory;
            _logger = logger;
            _orderModelFactory = orderModelFactory;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
            _nopHttpClient = nopHttpClient;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare dashboard model
        /// </summary>
        /// <param name="model">Dashboard model</param>
        /// <returns>Dashboard model</returns>
        public virtual DashboardModel PrepareDashboardModel(DashboardModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            var customer = _workContext.CurrentCustomer;
            model.UserProfileModel = new Models.Common.UserProfileModel();
            model.UserProfileModel.Email = _workContext.CurrentCustomer.Email;
            model.UserProfileModel.FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            model.UserProfileModel.LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);
            model.UserProfileModel.Gender = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.GenderAttribute);
            var dateOfBirth = _genericAttributeService.GetAttribute<DateTime?>(customer, NopCustomerDefaults.DateOfBirthAttribute);
            if (dateOfBirth.HasValue)
            {
                model.UserProfileModel.DateOfBirthDay = dateOfBirth.Value.Day;
                model.UserProfileModel.DateOfBirthMonth = dateOfBirth.Value.Month;
                model.UserProfileModel.DateOfBirthYear = dateOfBirth.Value.Year;
            }
            model.UserProfileModel.StreetAddress = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute);
            model.UserProfileModel.StreetAddress2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute);
            model.UserProfileModel.ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);
            model.UserProfileModel.City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute);
            model.UserProfileModel.CountryId = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountryIdAttribute);
            model.UserProfileModel.Phone = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);

            //prepare nested search models           _
            _commonModelFactory.PreparePopularSearchTermSearchModel(model.PopularSearchTerms);
            _orderModelFactory.PrepareBestsellerBriefSearchModel(model.BestsellersByAmount);
            _orderModelFactory.PrepareBestsellerBriefSearchModel(model.BestsellersByQuantity);

            return model;
        }

        /// <summary>
        /// Prepare nopCommerce news model
        /// </summary>
        /// <returns>nopCommerce news model</returns>
        public virtual NopCommerceNewsModel PrepareNopCommerceNewsModel()
        {
            var model = new NopCommerceNewsModel
            {
                HideAdvertisements = _adminAreaSettings.HideAdvertisementsOnAdminArea
            };

            try
            {
                //try to get news RSS feed
                var rssData = _staticCacheManager.Get(_cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.OfficialNewsModelKey), () =>
                {
                    try
                    {
                        return _nopHttpClient.GetNewsRssAsync().Result;
                    }
                    catch (AggregateException exception)
                    {
                        //rethrow actual excepion
                        throw exception.InnerException;
                    }
                });

                for (var i = 0; i < rssData.Items.Count; i++)
                {
                    var item = rssData.Items.ElementAt(i);
                    var newsItem = new NopCommerceNewsDetailsModel
                    {
                        Title = item.TitleText,
                        Summary = item.ContentText,
                        Url = item.Url.OriginalString,
                        PublishDate = item.PublishDate
                    };
                    model.Items.Add(newsItem);

                    //has new items?
                    if (i != 0)
                        continue;

                    var firstRequest = string.IsNullOrEmpty(_adminAreaSettings.LastNewsTitleAdminArea);
                    if (_adminAreaSettings.LastNewsTitleAdminArea == newsItem.Title)
                        continue;

                    _adminAreaSettings.LastNewsTitleAdminArea = newsItem.Title;
                    _settingService.SaveSetting(_adminAreaSettings);

                    //new item
                    if (!firstRequest)
                        model.HasNewItems = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("No access to the news. Website www.nopcommerce.com is not available.", ex);
            }

            return model;
        }

        #endregion
    }
}