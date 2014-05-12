using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Models
{
  public class SystemSettingsModel : AbstractDataModel<D_SystemSettings, SystemSettingsModel>
  {
    /// <summary>
    /// Процент проверочного платежа
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Range(typeof(Decimal), "0", "100", ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public decimal? CheckPaymentPercent { get; set; }
    /// <summary>
    /// Котировка
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public int? Quote { get; set; }

    public override SystemSettingsModel Bind(D_SystemSettings @object)
    {
      base.Bind(@object);

      CheckPaymentPercent = @object.CheckPaymentPercent;
      Quote = @object.Quote;

      return this;
    }

    public override D_SystemSettings UnBind(D_SystemSettings @object = null)
    {
      var d_systemSettings = base.UnBind(@object);

      if (CheckPaymentPercent == null)
        throw new ArgumentNullException("CheckPaymentPercent");

      d_systemSettings.CheckPaymentPercent = CheckPaymentPercent.Value;

      if (Quote == null)
        throw new ArgumentNullException("Quote");

      d_systemSettings.Quote = Quote.Value;

      return d_systemSettings;
    }
  }
}