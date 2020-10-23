using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Common
{

    public class UserProfileModel
    {
        [NopResourceDisplayName("Account.Fields.Gender")]
        public string Gender { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Account.Fields.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Account.Fields.WarehouseOwned")]
        public string WarehouseOwned { get; set; }

        [NopResourceDisplayName("Account.Fields.FirstName")]
        public string FirstName { get; internal set; }

        [NopResourceDisplayName("Account.Fields.LastName")]
        public string LastName { get; internal set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int DateOfBirthDay { get; internal set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int DateOfBirthMonth { get; internal set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int DateOfBirthYear { get; internal set; }

        [NopResourceDisplayName("Account.Fields.StreetAddress")]
        public string StreetAddress { get; internal set; }

        [NopResourceDisplayName("Account.Fields.StreetAddress2")]
        public string StreetAddress2 { get; internal set; }

        [NopResourceDisplayName("Account.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; internal set; }

        [NopResourceDisplayName("Account.Fields.City")]
        public string City { get; internal set; }
        public string CountryId { get; internal set; }
        [DataType(DataType.PhoneNumber)]
        [NopResourceDisplayName("Account.Fields.Phone")]
        public string Phone { get; internal set; }
    }
}
