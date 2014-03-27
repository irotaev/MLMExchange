using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MLMExchange.Models.Registration
{
  public class LoginModel : BaseModel
  {
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Login { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [StringLength(50, MinimumLength = 8, ErrorMessageResourceName = "FieldMinCharCountNotRequired_8", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Password { get; set; }
  }
}