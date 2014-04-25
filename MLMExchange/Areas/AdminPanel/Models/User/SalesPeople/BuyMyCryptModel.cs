using DataAnnotationsExtensions;
using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Areas.AdminPanel.Models.User.SalesPeople
{
  /// <summary>
  /// Купить my-crypt
  /// </summary>
  public class BuyMyCryptModel : BaseModel
  {
    /// <summary>
    /// Количество покупаемых my-crypt
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public int? MyCryptCount { get; set; }

    /// <summary>
    /// Комментарии
    /// </summary>
    public string Comment { get; set; }
  }
}
