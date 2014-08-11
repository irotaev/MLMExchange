using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate.Linq;
using Logic.Lib;

namespace Logic
{
  public class TradingSession : AbstractLogicObject<D_TradingSession>
  {
    public TradingSession(D_TradingSession tradingSession) : base(tradingSession) { }

    private static object _Locker = new { };

    public static explicit operator TradingSession(D_TradingSession dataTradingSession)
    {
      return new TradingSession(dataTradingSession);
    }

    /// <summary>
    /// Открытие торговой сессии
    /// </summary>
    /// <param name="buyingMyCryptRequest">Заявка на покупку MC</param>
    public static D_TradingSession OpenTradingSession(BuyingMyCryptRequest buyingMyCryptRequest)
    {
      if (buyingMyCryptRequest == null)
        throw new ArgumentNullException("buyingMyCryptRequest");

      if (buyingMyCryptRequest.BiddingParticipateApplication == null)
        throw new Logic.Lib.ApplicationException("Не задано свойство BiddingParticipateApplication у заявки на покупку MC");

      if (buyingMyCryptRequest.SystemSettings == null)
        throw new Logic.Lib.ApplicationException("Не задано своство SystemSettings у заявки на покупку MC");

      #region Создание торговой сессии
      D_TradingSession tradingSession = new D_TradingSession
      {
        BuyingMyCryptRequest = buyingMyCryptRequest,
        BiddingParticipateApplication = buyingMyCryptRequest.BiddingParticipateApplication,
        State = TradingSessionStatus.Open
      };
      #endregion

      #region Счет проверочного платежа
      D_Bill checkBill = new D_Bill
      {
        MoneyAmount = ((TradingSession)tradingSession).CalculateCheckPaymentMoneyAmount(),
        PaymentState = BillPaymentState.WaitingPayment,
        Payer = tradingSession.BuyingMyCryptRequest.Buyer
      };

      tradingSession.CheckBill = checkBill;
      #endregion

      #region Счет сбора продавцу
      D_Bill sallerInterestRateBill = new D_Bill
      {
        MoneyAmount = ((TradingSession)tradingSession).CalculateSallerInterestRateMoneyAmount(),
        Payer = tradingSession.BuyingMyCryptRequest.Buyer,
        PaymentState = BillPaymentState.WaitingPayment
      };

      tradingSession.SallerInterestRateBill = sallerInterestRateBill;
      #endregion

      #region Для продавца, все заявки на покупку в статусе ожидает подтверждение переводятся в состояние "Отозвано"
      {
        List<BuyingMyCryptRequest> buyingMyCryptRequests = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<BuyingMyCryptRequest>()
          .Where(x => x.State == BuyingMyCryptRequestState.AwaitingConfirm && x.Buyer.Id == buyingMyCryptRequest.SellerUser.Id).ToList();

        foreach (var request in buyingMyCryptRequests)
        {
          request.State = BuyingMyCryptRequestState.Recalled;
          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(buyingMyCryptRequest);
        }
      }
      #endregion

      #region Для продавца, перевожу все заявки на покупку для данной заявки продажи в состояние "Отменено"
      {
        List<BuyingMyCryptRequest> buyingMyCryptRequests = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<BuyingMyCryptRequest>()
          .Where(x => x.Id != buyingMyCryptRequest.Id && x.BiddingParticipateApplication.Id == buyingMyCryptRequest.BiddingParticipateApplication.Id).ToList();

        foreach (var request in buyingMyCryptRequests)
        {
          request.State = BuyingMyCryptRequestState.Denied;
          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(buyingMyCryptRequest);
        }
      }
      #endregion

      #region Для покупателя, все его заявки на покупку, которые ожидают подтверждения, перевожу в состояние "Отклонено"
      {
        List<BuyingMyCryptRequest> buyingMyCryptRequests = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<BuyingMyCryptRequest>()
          .Where(x => x.Id != buyingMyCryptRequest.Id && x.State == BuyingMyCryptRequestState.AwaitingConfirm && x.Buyer.Id == buyingMyCryptRequest.Buyer.Id).ToList();

        foreach (var request in buyingMyCryptRequests)
        {
          request.State = BuyingMyCryptRequestState.Recalled;
          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(buyingMyCryptRequest);
        }
      }
      #endregion

      #region Для покупателя, если имеется заявка на продажу, перевожу ее в статус "Отозвано"
      {
        D_BiddingParticipateApplication biddingApp = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<D_BiddingParticipateApplication>()
          .Where(x => x.Seller.Id == buyingMyCryptRequest.Buyer.Id && x.State == BiddingParticipateApplicationState.Filed).FirstOrDefault();

        if (biddingApp != null)
          ((BiddingParticipateApplication)biddingApp).TryChangeState(BiddingParticipateApplicationState.Recalled);
      }
      #endregion

      buyingMyCryptRequest.State = BuyingMyCryptRequestState.Accepted;
      buyingMyCryptRequest.BiddingParticipateApplication.State = BiddingParticipateApplicationState.Accepted;

      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(tradingSession);

      return tradingSession;
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
      decimal moneyAmount = (LogicObject.SystemSettings.CheckPaymentPercent / 100) * LogicObject.BuyingMyCryptRequest.MyCryptCount;

      return Bill.RoundBillMoneyAmount(moneyAmount);
    }

    /// <summary>
    /// Рассчитать количество денег, необходимое к выплате продавцу
    /// </summary>
    /// <returns>Количество денег</returns>
    public decimal CalculateSallerInterestRateMoneyAmount()
    {
      decimal moneyAmount = (1m / LogicObject.SystemSettings.Quote) * LogicObject.BuyingMyCryptRequest.MyCryptCount;

      return Bill.RoundBillMoneyAmount(moneyAmount);
    }

    /// <summary>
    /// Рассчитать прибыль покупателя my-crypt
    /// </summary>
    /// <returns></returns>
    public decimal CalculateBuyerProfit()
    {
      return (LogicObject.BuyingMyCryptRequest.MyCryptCount * (LogicObject.SystemSettings.ProfitPercent / 100));
    }

    #region Обеспечение доходности торговой сессии
    /// <summary>
    /// Оплачены ли счета доходности торговой сессии
    /// </summary>
    public bool IsYieldSessionBillsPaid
    {
      get
      {
        if (YieldSessionBillsNecessaryMoney() > 0)
          return false;

        foreach (var bill in _LogicObject.YieldSessionBills)
        {
          if (bill.PaymentState != BillPaymentState.Paid)
            return false;
        }

        return true;
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

      necessaryMoney = Bill.RoundBillMoneyAmount(necessaryMoney);

      return necessaryMoney;
    }

    /// <summary>
    /// Узнать оставшееся количество денег для выплаты прибыли покупателю.
    /// Считает только из платежей, занесенных в базу, на момент обращения к методу.
    /// Рассчитывается, лучше вызывать минимальное число раз
    /// </summary>
    /// <returns>Количество денег</returns>
    public decimal BuyerProfitNecessaryMoney()
    {
      decimal necessaryMoney = LogicObject.BuyingMyCryptRequest.MyCryptCount + CalculateBuyerProfit();

      IEnumerable<D_YieldSessionBill> d_bills = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
          .Query<D_YieldSessionBill>().Where(x => x.AcceptorTradingSession.Id == LogicObject.Id && x.PaymentAcceptor.Id == LogicObject.BuyingMyCryptRequest.Buyer.Id).ToList();

      foreach (var bill in d_bills)
      {
        necessaryMoney -= bill.MoneyAmount;
      }

      necessaryMoney = Bill.RoundBillMoneyAmount(necessaryMoney);

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

      lock (_Locker)
      {
        // Добавлен ли счет на оплату доходности торговой сессии
        bool isBillAdded = false;

        List<D_TradingSession> d_profitTradingSessions = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
          .Query<D_TradingSession>().Where(x => x.State == TradingSessionStatus.NeedProfit).OrderBy(x => x.DateLastYieldTradingSessionUnsureSearchRobotAddProfitBill).ToList();

        foreach (var profitSession in d_profitTradingSessions.Select(x => (TradingSession)x))
        {
          // Важен пересчет оставшейся суммы для обеспечения доходности сессии, т.к. 
          // может добавится счет в счета на обеспечение доходности текущей сессии
          decimal yieldSessionBillNecessaryMoney = YieldSessionBillsNecessaryMoney();
          decimal buyerProfitNecasseryMoney = profitSession.BuyerProfitNecessaryMoney();

          if (yieldSessionBillNecessaryMoney <= buyerProfitNecasseryMoney)
          {
            D_YieldSessionBill ensureBill = new D_YieldSessionBill
            {
              MoneyAmount = yieldSessionBillNecessaryMoney,
              Payer = _LogicObject.BuyingMyCryptRequest.Buyer,
              PaymentAcceptor = profitSession.LogicObject.BuyingMyCryptRequest.Buyer,
              PayerTradingSession = _LogicObject,
              AcceptorTradingSession = profitSession.LogicObject,
              PaymentState = BillPaymentState.WaitingPayment
            };

            Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(ensureBill);

            _LogicObject.YieldSessionBills.Add(ensureBill);

            profitSession.LogicObject.DateLastYieldTradingSessionUnsureSearchRobotAddProfitBill = DateTime.UtcNow;
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
              _NHibernateSession.SaveOrUpdate(profitSession.LogicObject);
            }
            else
            {
              _NHibernateSession.Delete(ensureBill);
            }

            profitSession.LogicObject.DateLastYieldTradingSessionUnsureSearchRobotAddProfitBill = DateTime.UtcNow;
            isBillAdded = true;
          }
        }

        #region Добавляю счета на оплату ДТС на имя системы (если поисковик долго не может найти реальных пользователей)
        {
          decimal sessionProgressMinutes = Application.Instance.CurrentSystemSettings.LogicObject.TradingSessionDuration;

          if (!isBillAdded
              &&
              ((LogicObject.DateLastYieldTradingSessionUnsureSearchRobotAddBill != null && LogicObject.DateLastYieldTradingSessionUnsureSearchRobotAddBill < DateTime.UtcNow.AddMinutes((double)-sessionProgressMinutes))
              || (LogicObject.DateLastYieldTradingSessionUnsureSearchRobotAddBill == null && LogicObject.CreationDateTime < DateTime.UtcNow.AddMinutes((double)-sessionProgressMinutes))))
          {
            //TODO:Rtv Добавить AcceptorTradingSession. Или подумать как решить данный вопрос
            D_YieldSessionBill ensureBill = new D_YieldSessionBill
            {
              MoneyAmount = YieldSessionBillsNecessaryMoney(),
              Payer = _LogicObject.BuyingMyCryptRequest.Buyer,
              PaymentAcceptor = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<D_System_User>().FirstOrDefault(),
              PayerTradingSession = _LogicObject,
              PaymentState = BillPaymentState.WaitingPayment
            };

            Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(ensureBill);
            _LogicObject.YieldSessionBills.Add(ensureBill);
            isBillAdded = true;
          }
        }
        #endregion

        if (isBillAdded)
          LogicObject.DateLastYieldTradingSessionUnsureSearchRobotAddBill = DateTime.UtcNow;

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(LogicObject);
      }
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

              ((User)LogicObject.BuyingMyCryptRequest.Buyer).TryPayRefererProfit(LogicObject);
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
