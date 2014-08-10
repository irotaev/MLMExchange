using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Models
{
  /// <summary>
  /// Модель системных настроек.
  /// </summary>
  public class SystemSettingsModel : AbstractDataModel<D_SystemSettings, SystemSettingsModel>
  {
    /// <summary>
    /// Процент проверочного платежа.
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Range(typeof(Decimal), "0", "999", ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n2}")]
    public decimal? CheckPaymentPercent { get; set; }

    /// <summary>
    /// Котировка
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Range(typeof(Int32), "0", "999", ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public int? Quote { get; set; }

    /// <summary>
    /// Длительность торговой сессии.
    /// Измеряется в минутах.
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Range(typeof(Decimal), "0", "9999", ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F}")]
    public decimal? TradingSessionDuration { get; set; }

    /// <summary>
    /// Процент доходности для продавца. Измеряется в процентах.
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Range(typeof(Decimal), "0", "999", ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F}")]
    public decimal? ProfitPercent { get; set; }

    /// <summary>
    /// Максимальное колличество mycrypto при заказе.
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [DataType(DataType.Text)]
    [Range(typeof(Int64), "0", "9999999", ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0}")]
    public long? MaxMyCryptCount { get; set; }

    /// <summary>
    /// Дефолтный системный пользователь
    /// </summary>
    internal UserModel DefaultSystemUser { get; set; }

    public string DefaultSystemUserName { get; set; }


    public override SystemSettingsModel Bind(D_SystemSettings @object)
    {
      base.Bind(@object);     

      CheckPaymentPercent = @object.CheckPaymentPercent;
      Quote = @object.Quote;
      TradingSessionDuration = @object.TradingSessionDuration;
      MaxMyCryptCount = @object.MaxMyCryptCount;
      ProfitPercent = @object.ProfitPercent;
      DefaultSystemUser = new UserModel().Bind(@object.DefaultSystemUser);
      DefaultSystemUserName = DefaultSystemUser.Login;

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

      if (TradingSessionDuration == null)
        throw new ArgumentNullException("TradingSessionDuration");

      d_systemSettings.TradingSessionDuration = TradingSessionDuration.Value;

      if(MaxMyCryptCount == null)
        throw new ArgumentNullException("MaxMyCryptCount");

      d_systemSettings.MaxMyCryptCount = MaxMyCryptCount.Value;

      if (ProfitPercent == null)
        throw new ArgumentNullException("ProfitPercent");

      d_systemSettings.ProfitPercent = ProfitPercent.Value;

      return d_systemSettings;
    }
  }
}