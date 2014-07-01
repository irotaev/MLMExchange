using Logic;
using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

namespace MLMExchange.Areas.AdminPanel.Models.PaymentSystem
{
  public interface IPaySellerInterestRateModel
  {
    /// <summary>
    /// Id торговой сессии, для оплаты комиссионного сбора продавца
    /// </summary>
    long? PaySellerInterestRateModel__TradeSessionId { get; set; }
  }

  /// <summary>
  /// Модель оплаты доходности торговой сесиии
  /// </summary>
  public interface IPayYieldTradingSessionBillModel
  {
    //long? IPayYieldTradingSessionBillModel__
  }

  /// <summary>
  /// Модель группы платежных систем
  /// </summary>
  public class PaymentSystemGroupModel : AbstractDataModel<PaymentSystemGroup, D_PaymentSystemGroup, PaymentSystemGroupModel>, IPaySellerInterestRateModel
  {
    /// <summary>
    /// Модели платежных систем типа банк
    /// </summary>
    public List<BankPaymentSystemModel> BankPaymentSystemModels { get; set; }
    /// <summary>
    /// Модель платёжных систем типа электронные
    /// </summary>
    public List<ElectronicPaymentSystemModel> ElectronicPaymentSystemModels { get; set; }

    #region Default PaymentSystem
    /// <summary>
    /// Дефолтная платежная система
    /// </summary>
    public BasePaymentSystemModel DefaultPaymentSystem { get; private set; }

    private string _DefaultPaymentSystemDisplayName = Logic.Properties.GeneralResources.EmptyPropertie;
    /// <summary>
    /// Имя отображения дефолтной платежной системы
    /// </summary>
    public string DefaultPaymentSystemDisplayName { get { return _DefaultPaymentSystemDisplayName; } }
    #endregion

    public override PaymentSystemGroupModel Bind(PaymentSystemGroup @object)
    {
      if (@object == null)
        throw new ArgumentNullException("object");

      base.Bind(@object);

      #region PaymentSystem
      Logic.PaymentSystem paymentSystem = @object.GetDefaultPaymentSystem();

      if (paymentSystem != null)
      {
        DefaultPaymentSystem = new BasePaymentSystemModel().Bind(paymentSystem);
        _DefaultPaymentSystemDisplayName = Logic.PaymentSystem.GetDisplayName(paymentSystem);
      }
      #endregion

      BankPaymentSystemModels = new List<BankPaymentSystemModel>();

      foreach (var bankSystem in @object.LogicObject.BankPaymentSystems)
      {
        BankPaymentSystemModel bankModel = new BankPaymentSystemModel().Bind(bankSystem);

        BankPaymentSystemModels.Add(bankModel);
      }

      ElectronicPaymentSystemModels = new List<ElectronicPaymentSystemModel>();

      foreach (var electronicSystem in @object.LogicObject.ElectronicPaymentSystems)
      {
        ElectronicPaymentSystemModel electronicModel = new ElectronicPaymentSystemModel().Bind(electronicSystem);

        ElectronicPaymentSystemModels.Add(electronicModel);
      }

      return this;
    }

    public override PaymentSystemGroup UnBind(PaymentSystemGroup @object = null)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Id торговой сессии, для оплаты комиссионного сбора продавца
    /// </summary>
    public long? PaySellerInterestRateModel__TradeSessionId { get; set; }
  }
}