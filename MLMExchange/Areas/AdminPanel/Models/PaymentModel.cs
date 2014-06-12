using Logic;
using MLMExchange.Areas.AdminPanel.Models.PaymentSystem;
using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

namespace MLMExchange.Areas.AdminPanel.Models
{
  /// <summary>
  /// Модель платежа.
  /// Базовая модель, без учета платежной системы
  /// <typeparam name="TPaymentModel">Тип модели платежа</typeparam>
  /// </summary>
  public abstract class AbstractPaymentModel<TPaymentModel> : AbstractDataModel<Payment, TPaymentModel>
    where TPaymentModel : AbstractPaymentModel<TPaymentModel>
  {
    /// <summary>
    /// Пользователь, который совершил платёж
    /// </summary>
    public UserModel Payer { get; set; }
    /// <summary>
    /// Количество денег по счету
    /// </summary>
    public decimal RealMoneyAmount { get; set; }

    public override TPaymentModel Bind(Payment @object)
    {
      base.Bind(@object);

      Payer = new UserModel().Bind(@object.Payer);
      RealMoneyAmount = @object.RealMoneyAmount;

      return (TPaymentModel)this;
    }

    public override Payment UnBind(Payment @object = null)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Модель банковского платежа
  /// </summary>
  public class BankPaymentModel : AbstractPaymentModel<BankPaymentModel>
  {
    public BankPaymentSystemModel BankPaymentSystemModel { get; set; }

    public override BankPaymentModel Bind(Payment @object)
    {
      #region BankPaymentSystem
      if (@object.PaymentSystem == null)
        throw new ApplicationException("Payment has no payment system");

      BankPaymentSystem bankPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<BankPaymentSystem>().Where(x => x.Id == @object.PaymentSystem.Id).FirstOrDefault();

      if (bankPaymentSystem == null)
        throw new ApplicationException("PaymentSystem in payment is not BankPaymentSystem type");

      BankPaymentSystemModel = new BankPaymentSystemModel().Bind(bankPaymentSystem);
      #endregion

      base.Bind(@object);

      return this;
    }

    public override Payment UnBind(Payment @object = null)
    {
      throw new NotImplementedException();
    }
  }
}