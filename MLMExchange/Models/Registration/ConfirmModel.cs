using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MLMExchange.Models.Registration
{
  public class ConfirmModel : AbstractModel
  {
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "FieldRangeCharCountNotRequired_6_20", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string ConfirmationCode { get; set; }

    public string PhoneNumber { get; set; }
  }
}