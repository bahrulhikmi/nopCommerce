using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    /// <summary>
    /// Represents a customer model
    /// </summary>
    public partial class EditMyProfileModel : BaseNopEntityModel
    {
        public EditMyProfileModel()        
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableTimeZones = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Username")]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Account.ChangePassword.Fields.NewPassword")]
        [DataType(DataType.Password)]
        [NoTrim] 
        public string Password { get; set; }

        [NopResourceDisplayName("Account.ChangePassword.Fields.ConfirmNewPassword")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string ConfirmPassword { get; set; }

        [NopResourceDisplayName("Account.ChangePassword.Fields.OldPassword")]
        [DataType(DataType.Password)]
        [NoTrim]
        public string OldPassword { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Gender")]
        public string Gender { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.StreetAddress")]
        public string StreetAddress { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.StreetAddress2")]
        public string StreetAddress2 { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.City")]
        public string City { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.County")]
        public string County { get; set; }


        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Country")]
        public int CountryId { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.StateProvince")]
        public int StateProvinceId { get; set; }

        public IList<SelectListItem> AvailableStates { get; set; }

        [DataType(DataType.PhoneNumber)]
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Phone")]
        public string Phone { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.TimeZoneId")]
        public string TimeZoneId { get; set; }

        public IList<SelectListItem> AvailableTimeZones { get; set; }

        public bool CountryEnabled { get; set; }

    }
}