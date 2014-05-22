using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

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

    /// <summary>
    /// Рассчитать количество денег, необходимое для проверочного платежа по данной сессии
    /// </summary>
    /// <returns>Количество денег</returns>
    public decimal CalculateCheckPaymentMoneyAmount()
    {
      return (LogicObject.SystemSettings.CheckPaymentPercent / 100) * LogicObject.BuyingMyCryptRequest.MyCryptCount;

    }

    /// <summary>
    /// Рассчитать количество денег, необходимое к выплате продавцу
    /// </summary>
    /// <returns>Количество денег</returns>
    public decimal CalculateSallerInterestRateMoneyAmount()
    {
      return ((decimal)1 / LogicObject.SystemSettings.Quote) * LogicObject.BuyingMyCryptRequest.MyCryptCount;
    }


    #region Обеспечение доходности торговой сессии
    /// <summary>
    /// Узнать оставшееся количество денег для оплаты доходности торговой сессии.
    /// Считает только из платежей, занесенных в базу, на момент обращения к методу
    /// </summary>
    /// <returns>Количество денег</returns>
    private decimal YieldSessionBillsNecessaryMoney()
    {

    }

    /// <summary>
    /// Обеспечить доходность торговой сессии.
    /// выставить счет
    /// </summary>
    internal void EnsureProfibility()
    {
      if (LogicObject.State != TradingSessionStatus.NeedEnsureProfibility)
        return;


    }
    #endregion
  }

  /// <summary>
  /// Список торговых сессий
  /// </summary>
  internal class TradingSessionList
  {
    /// <summary>
    /// Обеспечить доходность торговых сессий.
    /// Выставить счета
    /// </summary>
    internal void EnsureProfibilityOfTradingSessions()
    {
      List<D_TradingSession> d_tradingSessions = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_TradingSession>().Where(x => x.State == TradingSessionStatus.NeedEnsureProfibility).ToList();


    }

    /// <summary>
    /// Обеспечить доходность торговой сессии.
    /// Выставить счета
    /// </summary>
    private void EnsureProfibilityOfTradingSession(D_TradingSession d_tradingSession)
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
  }
}
