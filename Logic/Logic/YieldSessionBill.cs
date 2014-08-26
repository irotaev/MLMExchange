using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

namespace Logic
{
  /// <summary>
  /// Proxy-объект логики YieldSessionBill
  /// </summary>
  public class YieldSessionBill : Bill<D_YieldSessionBill>
  {
    public YieldSessionBill(D_YieldSessionBill d_baseObject) : base(d_baseObject) { }

    public static explicit operator YieldSessionBill(D_YieldSessionBill dataBaseObject)
    {
      return new YieldSessionBill(dataBaseObject);
    }

    public override bool TryChangePaymentState(BillPaymentState state)
    {
      bool result = base.TryChangePaymentState(state);

      switch(state)
      {
        case BillPaymentState.Paid:
          ((TradingSession)_LogicObject.PayerTradingSession).TryChangeStatus(TradingSessionStatus.WaitForProgressStart);
          break;
      }

      return result;
    }

    #region Replace bill
    /// <summary>
    /// Заменен ли счет другим счетом
    /// </summary>
    /// <returns></returns>
    public bool IsReplaced()
    {
      if (LogicObject.IsReplacedByPayer)
        return true;
      else
        return false;
    }

    /// <summary>
    /// Сделать счет замененным
    /// </summary>
    public void SetBillToReplacedState()
    {
      LogicObject.IsReplacedByPayer = true;

      if (LogicObject.AcceptorTradingSession.State == TradingSessionStatus.ProfitConfirmation)
        LogicObject.AcceptorTradingSession.State = TradingSessionStatus.NeedProfit;
    }
    #endregion

    /// <summary>
    /// Оплатить счет на доходность торговой сессии
    /// </summary>
    /// <param name="d_paymentSystem">Платежная система по которой, был совершен платеж</param>
    public void PayYieldSessionBill(D_PaymentSystem d_paymentSystem)
    {
      Payment payment = new Payment
      {
        Payer = LogicObject.Payer,
        PaymentSystem = d_paymentSystem,
        RealMoneyAmount = LogicObject.MoneyAmount
      };

      LogicObject.PaymentState = BillPaymentState.EnoughMoney;
      AddPayment(payment);

      _NHibernateSession.SaveOrUpdate(LogicObject);
    }
  }
}
