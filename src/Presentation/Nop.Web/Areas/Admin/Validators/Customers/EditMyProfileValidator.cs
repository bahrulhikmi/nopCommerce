using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Customers
{
    public class EditMyProfileValidator : BaseNopValidator<EditMyProfileModel>
    {
        public EditMyProfileValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {            
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(localizationService.GetResource("Account.ChangePassword.Fields.NewPassword.EnteredPasswordsDoNotMatch"));
        }
    }
}
