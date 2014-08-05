using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
  /// <summary>
  /// Прокси-объект счета.
  /// Используется для реализации наследования
  /// </summary>
  /// <typeparam name="TInheritObject">Объект данных, который является реализацией данных для
  /// текущего прокси-объекта, который наследует данный объект</typeparam>
  public class Bill<TInheritObject> : AbstractLogicObject<TInheritObject>
    where TInheritObject : D_Bill
  {
    public Bill(TInheritObject d_dataObject) : base(d_dataObject) { }

    /// <summary>
    /// Добавить платеж
    /// </summary>
    /// <param name="payment">Платеж</param>
    public virtual void AddPayment(Payment payment)
    {
      _LogicObject.GetPaymentList().Add(payment);
    }

    /// <summary>
    /// Попытаться поменять статус счета. 
    /// </summary>
    /// <param name="state">Новый статус</param>
    /// <returns>True - в случаи удачи, false - в случаи неудачи</returns>
    public virtual bool TryChangePaymentState(BillPaymentState state)
    {
      LogicObject.PaymentState = state;

      return true;
    }
  }

  public class Bill : Bill<D_Bill>
  {
    public Bill(D_Bill d_dataObject) : base(d_dataObject) { }

    public static explicit operator Bill(D_Bill dataBaseObject)
    {
      return new Bill(dataBaseObject);
    }

    /// <summary>
    /// Округлить сумму денег
    /// </summary>
    /// <param name="moneyAmount">Сумма денег</param>
    /// <returns>Округленная сумма</returns>
    public static decimal RoundBillMoneyAmount(decimal moneyAmount)
    {
      moneyAmount = moneyAmount >= 0 ? moneyAmount : 0;
      moneyAmount = moneyAmount != 0 ? Math.Floor(moneyAmount) + 1 : 0;

      return moneyAmount;
    }

    /// <summary>
    /// Оплатить проверочный платеж    
    /// <param name="payer">Плательщик</param>
    /// </summary>
    public void PayCheckBill(D_User payer)
    {
      Payment checkPayment = new Payment
      {
        RealMoneyAmount = LogicObject.MoneyAmount,
        Payer = payer,
        Bill = LogicObject
      };

      AddPayment(checkPayment);

      //TODO:Rtv Прикрепить платежную систему
      LogicObject.PaymentState = BillPaymentState.Paid;

      _NHibernateSession.SaveOrUpdate(LogicObject);
    }

    /// <summary>
    /// Заплатить комиссионный платеж продавцу
    /// <param name="payer">Плательщик</param>
    /// <param name="paymentSystem">Платежная система продавца</param>
    /// </summary>
    public void PaySellerInterestBill(D_User payer, D_PaymentSystem paymentSystem)
    {
      Payment sallerInterestRatePayment = new Payment
      {
        RealMoneyAmount = LogicObject.MoneyAmount,
        Payer = payer,
        PaymentSystem = paymentSystem,
        Bill = LogicObject
      };

      AddPayment(sallerInterestRatePayment);
    }
  }
}
