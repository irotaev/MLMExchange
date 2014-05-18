using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Logic
{
  public class TradingSession : AbstractLogicObject<D_TradingSession>
  {
    public TradingSession(D_TradingSession tradingSession) : base(tradingSession) { }

    public static explicit operator TradingSession(D_TradingSession dataTradingSession)
    {
      return new TradingSession(dataTradingSession);
    }

    /// <summary>
    /// Обеспечить доходность торговой сессии.
    /// Выставить счета
    /// </summary>
    public void EnsureProfibilityOfTradingSession()
    {
      if (_LogicObject.State != TradingSessionStatus.Open || _LogicObject.CheckBill.PaymentState != BillPaymentState.Paid || _LogicObject.SallerInterestRateBill.PaymentState != BillPaymentState.Paid)
        throw new ApplicationException("This session is not ready to ensure profibillity");

      if (_LogicObject.YieldSessionBills.Count > 0)
        return;

      //TODO:Rtv переделать
      for (int index = 0; index < 5; index++)
      {
        D_YieldSessionBill ensureBill = new D_YieldSessionBill
        {
          MoneyAmount = 135,
          Payer = _LogicObject.BuyingMyCryptRequest.Buyer,
          PaymentAcceptor = _LogicObject.BuyingMyCryptRequest.Buyer, //TODO:Rtv переделать
          TradingSession = _LogicObject
        };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(ensureBill);

        _LogicObject.YieldSessionBills.Add(ensureBill);
      }
    }

    /// <summary>
    /// Оплачены ли счета доходности торговой сессии
    /// </summary>
    public bool IsYieldSessionBillsPaid 
    {
      get
      {
        bool billsPaid = true;

        foreach(var bill in _LogicObject.YieldSessionBills)
        {
          billsPaid = bill.PaymentState == BillPaymentState.Paid;
        }

        return billsPaid;
      }
    }
  }
}
