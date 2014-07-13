using Logic;
using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Models.PaymentSystem
{
  /// <summary>
  /// Модель счета
  /// </summary>
  public class BillModel : AbstractDataModel<Bill, D_Bill, BillModel>
  {
    /// <summary>
    /// Кому выставлен счет. 
    /// Пользователь, который должен оплатить счет
    /// </summary>
    public UserModel User { get; set; }
    /// <summary>
    /// Пользователь, которому оплачивают счет.
    /// Тот, кто принемает платеж. Необязательный параметр
    /// </summary>
    public UserModel PaymentAcceptor { get; set; }
    /// <summary>
    /// Количество денег по счету
    /// </summary>
    public decimal? MoneyAmount { get; set; }
    /// <summary>
    /// Платежы по счету
    /// </summary>
    public IEnumerable<BasePaymentModel> Payments { get; set; }
    /// <summary>
    /// Id платежей по счету
    /// </summary>
    public IEnumerable<long> PaymentIds
    {
      get
      {
        if (Payments == null)
        {
          return null;
        }
        else
        {
          return Payments.Select(x => x.Id.Value);
        }
      }
    }
    /// <summary>
    /// Состояние счета
    /// </summary>
    public BillPaymentState BillPaymentState { get; set; }
    
    public override BillModel Bind(Bill @object)
    {
      if (@object == null)
        throw new ArgumentNullException("object");

      base.Bind(@object);

      User = new UserModel().Bind(@object.LogicObject.Payer);

      if (@object.LogicObject.PaymentAcceptor != null)
        PaymentAcceptor = new UserModel().Bind(@object.LogicObject.PaymentAcceptor);

      MoneyAmount = @object.LogicObject.MoneyAmount;

      Payments = @object.LogicObject.Payments.Select(x => new BasePaymentModel().Bind(x));

      BillPaymentState = @object.LogicObject.PaymentState;

      return this;
    }

    public override Bill UnBind(Bill @object = null)
    {
      throw new NotImplementedException();
    }
  }
}