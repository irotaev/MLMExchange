using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Models.Registration
{
  public class UserModel : BaseModel
  {
    [HiddenInput(DisplayValue = false)]
    public long Id { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Login { get; set; }
      
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [StringLength(50, MinimumLength = 8, ErrorMessageResourceName = "FieldMinCharCountNotRequired_8", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [System.Web.Mvc.Compare("Password", ErrorMessageResourceName = "PasswordDoesnotMatchWithPasswordConfirmation", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [StringLength(50, MinimumLength = 8, ErrorMessageResourceName = "FieldMinCharCountNotRequired_8", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [DataType(DataType.Password)]
    public string PasswordConfirmation { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Name { get; set; }

    public string Surname { get; set; }

    public string Patronymic { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessageResourceName = "FieldEmailInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Email { get; set; }

    public HttpPostedFileBase Photo { get; set; }
  }
}