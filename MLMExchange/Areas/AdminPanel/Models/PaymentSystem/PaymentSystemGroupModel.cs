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
  public class PaymentSystemGroupModel : AbstractDataModel<PaymentSystemGroup, PaymentSystemGroupModel>, IPaySellerInterestRateModel
  {
    /// <summary>
    /// Модели платежных систем типа банк
    /// </summary>
    public List<BankPaymentSystemModel> BankPaymentSystemModels { get; set; }
    public bool IsHasDefaultPaymentSystem { get; set; }

    public override PaymentSystemGroupModel Bind(PaymentSystemGroup @object)
    {
      if (@object == null)
        throw new ArgumentNullException("object");

      base.Bind(@object);

      Logic.PaymentSystem defaultPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<Logic.PaymentSystem>().Where(x => x.PaymentSystemGroup.Id == @object.Id && x.IsDefault == true).FirstOrDefault();

      IsHasDefaultPaymentSystem = defaultPaymentSystem != null;

      BankPaymentSystemModels = new List<BankPaymentSystemModel>();

      foreach(var bankSystem in @object.BankPaymentSystems)
      {
        BankPaymentSystemModel bankModel = new BankPaymentSystemModel().Bind(bankSystem);

        BankPaymentSystemModels.Add(bankModel);
      }

      return this;
    }

    public override D_BaseObject UnBind(D_BaseObject @object = null)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Id торговой сессии, для оплаты комиссионного сбора продавца
    /// </summary>
    public long? PaySellerInterestRateModel__TradeSessionId { get; set; }
  }
}