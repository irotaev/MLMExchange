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
    /// Платежи, по обеспечению прибыли покупателя
    /// </summary>
    public IList<Payment> BuyerProfitPayments
    {
      get
      {
        return Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
          .Query<D_YieldSessionBill>().Where(x => x.PaymentAcceptor.Id == LogicObject.BuyingMyCryptRequest.Buyer.Id && !x.IsNeedSubstantialMoney).SelectMany(x => x.Payments).ToList();
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

        foreach (var bill in _LogicObject.YieldSessionBills)
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
      return (1m / LogicObject.SystemSettings.Quote) * LogicObject.BuyingMyCryptRequest.MyCryptCount;
    }

    /// <summary>
    /// Рассчитать прибыль покупателя my-crypt
    /// </summary>
    /// <returns></returns>
    public decimal CalculateBuyerProfit()
    {
      return LogicObject.BuyingMyCryptRequest.MyCryptCount * (1m / LogicObject.SystemSettings.Quote);
    }


    #region Обеспечение доходности торговой сессии
    /// <summary>
    /// Узнать оставшееся количество денег для оплаты доходности торговой сессии.
    /// Считает только из счетов, занесенных в базу, на момент обращения к методу
    /// </summary>
    /// <returns>Количество денег</returns>
    private decimal YieldSessionBillsNecessaryMoney()
    {
      decimal necessaryMoney = LogicObject.BuyingMyCryptRequest.MyCryptCount - CalculateCheckPaymentMoneyAmount() - CalculateSallerInterestRateMoneyAmount();

      foreach (var yieldBill in LogicObject.YieldSessionBills)
      {
        necessaryMoney -= yieldBill.MoneyAmount;
      }

      return necessaryMoney;
    }

    /// <summary>
    /// Узнать оставшееся количество денег для выплаты прибыли покупателю.
    /// Считает только из платежей, занесенных в базу, на момент обращения к методу.
    /// Рассчитывается, лучше вызывать минимальное число раз
    /// </summary>
    /// <returns>Количество денег</returns>
    private decimal BuyerProfitNecessaryMoney()
    {
      decimal necessaryMoney = CalculateBuyerProfit();

      foreach(var payment in BuyerProfitPayments)
      {
        necessaryMoney -= payment.RealMoneyAmount;
      }

      return necessaryMoney;
    }

    /// <summary>
    /// Обеспечить доходность торговой сессии.
    /// Выставить счет
    /// </summary>
    internal void EnsureProfibility()
    {
      if (LogicObject.State != TradingSessionStatus.NeedEnsureProfibility)
        return;      

      List<D_TradingSession> d_profitTradingSessions = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_TradingSession>().Where(x => x.State == TradingSessionStatus.NeedProfit).ToList();

      foreach(var profitSession in d_profitTradingSessions.Select(x => (TradingSession)x))
      {
        // Важен пересчет оставшейся суммы для обеспечения доходности сессии, т.к. 
        // может добавится счет в счета на обеспечение доходности текущей сессии
        decimal yieldSessionBullNecessaryMoney = YieldSessionBillsNecessaryMoney();
        decimal buyerProfitNecasseryMoney = profitSession.BuyerProfitNecessaryMoney();        

        if (yieldSessionBullNecessaryMoney <= buyerProfitNecasseryMoney)
        {
          D_YieldSessionBill ensureBill = new D_YieldSessionBill
          {
            MoneyAmount = yieldSessionBullNecessaryMoney,
            Payer = _LogicObject.BuyingMyCryptRequest.Buyer,
            PaymentAcceptor = profitSession.LogicObject.BiddingParticipateApplication.Seller,
            TradingSession = _LogicObject
          };

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(ensureBill);

          _LogicObject.YieldSessionBills.Add(ensureBill);

          break;
        }
        else
        {
          D_YieldSessionBill ensureBill = new D_YieldSessionBill
          {
            MoneyAmount = buyerProfitNecasseryMoney,
            Payer = _LogicObject.BuyingMyCryptRequest.Buyer,
            PaymentAcceptor = profitSession.LogicObject.BiddingParticipateApplication.Seller,
            TradingSession = _LogicObject
          };

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(ensureBill);

          _LogicObject.YieldSessionBills.Add(ensureBill);
        }
      }
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

      foreach(var tradingSession in d_tradingSessions.Select(x => (TradingSession)x))
      {
        tradingSession.EnsureProfibility();
      }
    }
  }
}
