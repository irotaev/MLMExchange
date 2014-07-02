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
  /// Базовый интерфейс платежной системы
  /// </summary>
  public interface IBasePaymentModel
  {
    long? Id { get; }
    DateTime CreationDateTime { get; }
    /// <summary>
    /// Пользователь, который совершил платёж
    /// </summary>
    UserModel Payer { get; }
    /// <summary>
    /// Количество денег по счету
    /// </summary>
    decimal RealMoneyAmount { get; }
    /// <summary>
    /// Тип платежа
    /// </summary>
    PaymentType Type { get; }
  }

  /// <summary>
  /// Тип платежа
  /// </summary>
  public enum PaymentType : int
  {
    /// <summary>
    /// Банк
    /// </summary>
    Bank,
    /// <summary>
    /// Электронный платеж
    /// </summary>
    Electronic
  }

  /// <summary>
  /// Модель платежа.
  /// Базовая модель, без учета платежной системы
  /// <typeparam name="TPaymentModel">Тип модели платежа</typeparam>
  /// </summary>
  public abstract class AbstractPaymentModel<TPaymentModel> : AbstractDataModel<Payment, TPaymentModel>, IBasePaymentModel
    where TPaymentModel : AbstractPaymentModel<TPaymentModel>
  {
    /// <summary>
    /// Пользователь, который совершил платёж
    /// </summary>
    public UserModel Payer { get; private set; }
    /// <summary>
    /// Количество денег по счету
    /// </summary>
    public decimal RealMoneyAmount { get; private set; }

    public PaymentType Type { get; protected set; }

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
  /// Модель базавого платежа.
  /// Базовая модель, без учета платежной системы
  /// </summary>
  public class BasePaymentModel : AbstractPaymentModel<BasePaymentModel> { }

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

      D_BankPaymentSystem bankPaymentSystem;

      if (@object.PaymentSystem is D_BankPaymentSystem)
      {
        BankPaymentSystemModel = new BankPaymentSystemModel().Bind((D_BankPaymentSystem)@object.PaymentSystem);
      }
      else
      {
        bankPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
          .Query<D_BankPaymentSystem>().Where(x => x.Id == @object.PaymentSystem.Id).FirstOrDefault();

        if (bankPaymentSystem == null)
          throw new ApplicationException("PaymentSystem in payment is not BankPaymentSystem type");

        BankPaymentSystemModel = new BankPaymentSystemModel().Bind(bankPaymentSystem);
      }
      #endregion

      base.Bind(@object);

      return this;
    }

    public override Payment UnBind(Payment @object = null)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Модель электронного платежа
  /// </summary>
  public class ElectronicPaymentModel : AbstractPaymentModel<ElectronicPaymentModel>
  {
    public ElectronicPaymentSystemModel ElectronicPaymentSystemModel { get; set; }

    public override ElectronicPaymentModel Bind(Payment @object)
    {
      if(@object.PaymentSystem == null)
        throw new ApplicationException("Payment has no payment system");

      D_ElectronicPaymentSystem electronicPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_ElectronicPaymentSystem>().Where(x => x.Id == @object.PaymentSystem.Id).FirstOrDefault();

      if (electronicPaymentSystem == null)
        throw new ApplicationException("PaymentSystem in payment is not ElectronicPaymentSystem type");

      ElectronicPaymentSystemModel = new ElectronicPaymentSystemModel().Bind(electronicPaymentSystem);

      base.Bind(@object);

      return this;
    }

    public override Payment UnBind(Payment @object = null)
    {
      throw new NotImplementedException();
    }
  }
}