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
    /// Получить счета прибыли текущей торговой сессии
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Bill> GetNeedPaymentBills()
    {
      IList<D_YieldSessionBill> bills = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_YieldSessionBill>().Where(x => x.AcceptorTradingSession.Id == LogicObject.Id && x.PaymentAcceptor.Id == LogicObject.BuyingMyCryptRequest.Buyer.Id).ToList();

      return bills.Select(x => (Bill)x);
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
      return LogicObject.BuyingMyCryptRequest.MyCryptCount + (LogicObject.BuyingMyCryptRequest.MyCryptCount * (LogicObject.SystemSettings.ProfitPercent / 100));
    }

    #region Обеспечение доходности торговой сессии
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
          billsPaid = bill.PaymentState == BillPaymentState.EnoughMoney;
        }

        return billsPaid;
      }
    }

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

      IEnumerable<D_YieldSessionBill> d_bills = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
          .Query<D_YieldSessionBill>().Where(x => x.AcceptorTradingSession.Id == LogicObject.Id && x.PaymentAcceptor.Id == LogicObject.BuyingMyCryptRequest.Buyer.Id).ToList();

      foreach (var bill in d_bills)
      {
        necessaryMoney -= bill.MoneyAmount;
      }

      return necessaryMoney;
    }

    /// <summary>
    /// Обеспечить доходность торговой сессии.
    /// Выставить счет
    /// </summary>
    internal void EnsureProfibility()
    {
      if (LogicObject.State != TradingSessionStatus.NeedEnsureProfibility || YieldSessionBillsNecessaryMoney() == 0)
        return;

      // Добавлен ли счет на оплату доходности торговой сессии
      bool isBillAdded = false;

      List<D_TradingSession> d_profitTradingSessions = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_TradingSession>().Where(x => x.State == TradingSessionStatus.NeedProfit).ToList();

      foreach (var profitSession in d_profitTradingSessions.Select(x => (TradingSession)x))
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
            PaymentAcceptor = profitSession.LogicObject.BuyingMyCryptRequest.Buyer,
            PayerTradingSession = _LogicObject,
            AcceptorTradingSession = profitSession.LogicObject,
            PaymentState = BillPaymentState.WaitingPayment
          };

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(ensureBill);

          _LogicObject.YieldSessionBills.Add(ensureBill);
          isBillAdded = true;

          break;
        }
        else
        {
          D_YieldSessionBill ensureBill = new D_YieldSessionBill
          {
            MoneyAmount = buyerProfitNecasseryMoney,
            Payer = _LogicObject.BuyingMyCryptRequest.Buyer,
            PaymentAcceptor = profitSession.LogicObject.BuyingMyCryptRequest.Buyer,
            PayerTradingSession = _LogicObject,
            AcceptorTradingSession = profitSession.LogicObject,
            PaymentState = BillPaymentState.WaitingPayment
          };

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(ensureBill);
          _LogicObject.YieldSessionBills.Add(ensureBill);

          if (profitSession.TryChangeStatus(TradingSessionStatus.ProfitConfirmation))
          {
            _NhibernateSession.SaveOrUpdate(profitSession.LogicObject);
          }
          else
          {
            _NhibernateSession.Delete(ensureBill);
          }

          isBillAdded = true;
        }
      }

      #region Добавляю счета на оплату ДТС на имя системы (если поисковик долго не может найти реальных пользователей)
      if (!isBillAdded
          &&
          ((LogicObject.DateLastYieldTradingSessionUnsureSearchRobotAddBill != null && LogicObject.DateLastYieldTradingSessionUnsureSearchRobotAddBill < DateTime.UtcNow.AddSeconds(-30))
          || (LogicObject.DateLastYieldTradingSessionUnsureSearchRobotAddBill == null && LogicObject.CreationDateTime < DateTime.UtcNow.AddSeconds(-30))))
      {
        //TODO:Rtv Добавить AcceptorTradingSession. Или подумать как решить данный вопрос
        D_YieldSessionBill ensureBill = new D_YieldSessionBill
        {
          MoneyAmount = YieldSessionBillsNecessaryMoney() > 100 ? YieldSessionBillsNecessaryMoney() / 2 : YieldSessionBillsNecessaryMoney(),
          Payer = _LogicObject.BuyingMyCryptRequest.Buyer,
          PaymentAcceptor = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<D_System_User>().FirstOrDefault(),
          PayerTradingSession = _LogicObject,
          PaymentState = BillPaymentState.WaitingPayment
        };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(ensureBill);
        _LogicObject.YieldSessionBills.Add(ensureBill);
        isBillAdded = true;
      }
      #endregion

      if (isBillAdded)
        LogicObject.DateLastYieldTradingSessionUnsureSearchRobotAddBill = DateTime.UtcNow;

      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(LogicObject);
    }
    #endregion

    /// <summary>
    /// Попробовать изменить статус торговой сессии
    /// </summary>
    /// <param name="state">На какой статус надо изменить</param>
    /// <returns>Успех операции</returns>
    public bool TryChangeStatus(TradingSessionStatus state)
    {
      switch (state)
      {
        case TradingSessionStatus.WaitForProgressStart:
          if (_LogicObject.State == TradingSessionStatus.NeedEnsureProfibility && IsYieldSessionBillsPaid)
          {
            _LogicObject.State = TradingSessionStatus.WaitForProgressStart;
            return true;
          }
          break;

        case TradingSessionStatus.SessionInProgress:
          if (_LogicObject.State == TradingSessionStatus.WaitForProgressStart)
          {
            _LogicObject.State = TradingSessionStatus.SessionInProgress;
            _LogicObject.ClosingSessionDateTime = DateTime.UtcNow.AddMinutes((double)_LogicObject.SystemSettings.TradingSessionDuration);
            return true;
          }
          break;

        case TradingSessionStatus.NeedProfit:
          if (LogicObject.State == TradingSessionStatus.SessionInProgress)
          {
            if (DateTime.UtcNow >= LogicObject.ClosingSessionDateTime.Value)
              LogicObject.State = TradingSessionStatus.NeedProfit;
            return true;
          }
          break;

        case TradingSessionStatus.ProfitConfirmation:
          if (LogicObject.State == TradingSessionStatus.NeedProfit)
          {
            //TODO:Rtv получается двойное обращение к базе (BuyerProfitNecessaryMoney тут и, скорее всего, до вызова изменения состояния)
            if (BuyerProfitNecessaryMoney() == 0)
            {
              _LogicObject.State = TradingSessionStatus.ProfitConfirmation;
              return true;
            }
          }
          break;

        case TradingSessionStatus.Closed:
          if (LogicObject.State == TradingSessionStatus.ProfitConfirmation)
          {
            if (GetNeedPaymentBills().All(x => x.LogicObject.PaymentState == BillPaymentState.Paid))
            {
              LogicObject.State = TradingSessionStatus.Closed;
              return true;
            }
          }
          break;
      }

      return false;
    }
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

      foreach (var tradingSession in d_tradingSessions.Select(x => (TradingSession)x))
      {
        tradingSession.EnsureProfibility();
      }
    }
  }
}
