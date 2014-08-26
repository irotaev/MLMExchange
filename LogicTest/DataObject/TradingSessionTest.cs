using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using NHibernate.Proxy;
using Microsoft.Practices.Unity;

namespace LogicTest.DataObject
{
  [TestClass]
  public class TradingSessionTest : AbstractTest
  {
    /// <summary>
    /// Создать торговую сессию в состоянии открыта
    /// </summary>
    /// <returns>Созданная торговая сессия</returns>
    public static D_TradingSession CreateTradingSession_State_Open(ulong biddingMyCryptCount = 2000, ulong buyingRequestMyCryptCount = 1000)
    {
      D_User seller = D_UserTest.CreateUser();
      D_User buyer = D_UserTest.CreateUser(false);

      #region Назначаю рефералов
      buyer.RefererRole = ((User)seller).GetRole<D_UserRole>();

      _NHibernaetSession.SaveOrUpdate(buyer);
      #endregion

      _NHibernaetSession.SaveOrUpdate(seller);
      _NHibernaetSession.SaveOrUpdate(buyer);

      D_BiddingParticipateApplication participateApplication = BiddingParticipateApplicationTest.CreateBiddingParticipateApplication(seller, (long)biddingMyCryptCount);

      D_SystemSettings systemSettings = new D_SystemSettings
      {
        CheckPaymentPercent = 3,
        MaxMyCryptCount = 10000,
        ProfitPercent = 15,
        Quote = 8,
        TradingSessionDuration = 0.01m, // Не менять. Настроин поток синхронизации.
        RootReferer = _NHibernaetSession.Query<D_User>().Where(x => x.Login == "administrator_irotaev").First()
      };

      _NHibernaetSession.SaveOrUpdate(systemSettings);

      BuyingMyCryptRequest buyingRequest = BuyingMyCryptRequestTest.CreateBuyingMyCryptRequest(buyer, participateApplication, systemSettings, (long)buyingRequestMyCryptCount);

      D_TradingSession d_tradingSession = TradingSession.OpenTradingSession(buyingRequest);

      _NHibernaetSession.Transaction.Commit();
      _NHibernaetSession.BeginTransaction();

      _NHibernaetSession.Refresh(d_tradingSession);

      Assert.IsTrue(d_tradingSession != null, "Не получена торговая сессия как результат открытия сессии");

      return d_tradingSession;
    }
    /// <summary>
    /// Создать торговую сессию в состоянии открыта
    /// </summary>
    /// <param name="buyingMyCryptRequest">Заявка на покупку MC</param>
    /// <returns>Созданная торговая сессия</returns>
    public static D_TradingSession CreateTradingSession_State_Open(BuyingMyCryptRequest buyingMyCryptRequest)
    {
      D_TradingSession d_tradingSession = TradingSession.OpenTradingSession(buyingMyCryptRequest);

      Assert.IsTrue(d_tradingSession != null, "Не получена торговая сессия как результат открытия сессии");

      return d_tradingSession;
    }

    /// <summary>
    /// Перевести торговую сессию в состояние ждет удовлетворение доходности торговой сессии
    /// </summary>
    public static D_TradingSession ChangeState_To_NeedEnsureProfibility(D_TradingSession tradingSession)
    {
      if (tradingSession == null)
        throw new TestParametrNullException("tradingSession");

      if (tradingSession.State != TradingSessionStatus.Open)
        throw new TestException("Торговая сессия может быть переведена в состояние 'Need ensure profibility' только из состояния open");

      ((Bill)tradingSession.CheckBill).PayCheckBill(tradingSession.BuyingMyCryptRequest.Buyer);
      ((Bill)tradingSession.SallerInterestRateBill).PaySellerInterestBill(tradingSession.BuyingMyCryptRequest.Buyer,
        tradingSession.BuyingMyCryptRequest.SellerUser.PaymentSystemGroup.BankPaymentSystems.FirstOrDefault());

      tradingSession.SallerInterestRateBill.PaymentState = BillPaymentState.Paid;
      tradingSession.BiddingParticipateApplication.State = BiddingParticipateApplicationState.Closed;
      ((BiddingParticipateApplication)tradingSession.BiddingParticipateApplication).WriteOfBuyedMyCrypt();

      tradingSession.State = TradingSessionStatus.NeedEnsureProfibility;

      _NHibernaetSession.SaveOrUpdate(tradingSession);

      return tradingSession;
    }

    public static D_TradingSession ChangeState_To_WaitForProgressStart(D_TradingSession tradingSession)
    {
      if (tradingSession == null)
        throw new TestParametrNullException("tradingSession");

      if (tradingSession.State != TradingSessionStatus.NeedEnsureProfibility)
        throw new TestException("Торговая сессия может быть переведена в состояние 'WaitProgressStart' только из состояния 'Need ensure profibility'");

      D_TradingSession acceptedTradingSession = TradingSessionTest.CreateTradingSession_State_Open();
      acceptedTradingSession.State = TradingSessionStatus.ProfitConfirmation;

      _NHibernaetSession.SaveOrUpdate(acceptedTradingSession);

      tradingSession.YieldSessionBills.Add(new D_YieldSessionBill
      {
        //TODO:Rtv доделать
        AcceptorTradingSession = acceptedTradingSession,
        PaymentAcceptor = acceptedTradingSession.BuyingMyCryptRequest.Buyer,
        MoneyAmount = 1000,
        Payer = tradingSession.BuyingMyCryptRequest.Buyer,
        PayerTradingSession = tradingSession,
        PaymentState = BillPaymentState.WaitingPayment
      });

      ((YieldSessionBill)tradingSession.YieldSessionBills.FirstOrDefault())
        .PayYieldSessionBill(acceptedTradingSession.BuyingMyCryptRequest.Buyer.PaymentSystemGroup.BankPaymentSystems.FirstOrDefault());

      _NHibernaetSession.Transaction.Commit();
      _NHibernaetSession.BeginTransaction();

      // Покупатель подтверждает оплату счета. Т.е. он увидел деньги по этому счету
      ((YieldSessionBill)tradingSession.YieldSessionBills.FirstOrDefault()).TryChangePaymentState(BillPaymentState.Paid);
      tradingSession.State = TradingSessionStatus.WaitForProgressStart;

      _NHibernaetSession.SaveOrUpdate(tradingSession);

      return tradingSession;
    }

    public static D_TradingSession ChangeState_To_SessionInProgress(D_TradingSession tradingSession)
    {
      if (tradingSession == null)
        throw new TestParametrNullException("tradingSession");

      if (tradingSession.State != TradingSessionStatus.WaitForProgressStart)
        throw new TestException("Торговая сессия может быть переведена в состояние 'SessionInProgress' только из состояния 'WaitProgressStart'");

      ((TradingSession)tradingSession).TryChangeStatus(TradingSessionStatus.SessionInProgress);

      _NHibernaetSession.SaveOrUpdate(tradingSession);

      return tradingSession;
    }

    public static D_TradingSession ChangeState_To_NeedProfit(D_TradingSession tradingSession)
    {
      if (tradingSession == null)
        throw new TestParametrNullException("tradingSession");

      if (tradingSession.State != TradingSessionStatus.SessionInProgress)
        throw new TestException("Торговая сессия может быть переведена в состояние 'NeedProfit' только из состояния 'SessionInProgress'");

      System.Threading.Thread.Sleep((int)(tradingSession.SystemSettings.TradingSessionDuration * 60 * 1000));

      ((TradingSession)tradingSession).TryChangeStatus(TradingSessionStatus.NeedProfit);

      return tradingSession;
    }

    public static D_TradingSession ChangeState_To_ProfitConfirmation(D_TradingSession tradingSession, ushort profitBillCount = 10)
    {
      if (tradingSession == null)
        throw new TestParametrNullException("tradingSession");

      if (tradingSession.State != TradingSessionStatus.NeedProfit)
        throw new TestException("Торговая сессия может быть переведена в состояние 'ProfitConfirmation' только из состояния 'NeedProfit'");

      D_TradingSession payerProfitSession = TradingSessionTest.CreateTradingSession_State_Open();

      #region Создание платежей на прибыль текущей сессии
      decimal buyerProfit = tradingSession.BuyingMyCryptRequest.MyCryptCount + ((TradingSession)tradingSession).CalculateBuyerProfit();

      decimal billMoneyAmount = Bill.RoundBillMoneyAmount(buyerProfit / profitBillCount);

      for (uint index = 1; index <= 10; index++)
      {
        D_YieldSessionBill yieldSessionBill = new D_YieldSessionBill
        {
          AcceptorTradingSession = tradingSession,
          MoneyAmount = billMoneyAmount,
          Payer = payerProfitSession.BuyingMyCryptRequest.Buyer,
          PayerTradingSession = payerProfitSession,
          PaymentAcceptor = tradingSession.BuyingMyCryptRequest.Buyer,
          PaymentState = BillPaymentState.EnoughMoney
        };

        ((Bill)yieldSessionBill).AddPayment(new Payment
        {
          Bill = yieldSessionBill,
          PaymentSystem = payerProfitSession.BuyingMyCryptRequest.Buyer.PaymentSystemGroup.BankPaymentSystems.FirstOrDefault(),
          RealMoneyAmount = billMoneyAmount,
          Payer = payerProfitSession.BuyingMyCryptRequest.Buyer
        });

        _NHibernaetSession.SaveOrUpdate(yieldSessionBill);
      }
      #endregion

      ((TradingSession)tradingSession).TryChangeStatus(TradingSessionStatus.ProfitConfirmation);

      payerProfitSession.State = TradingSessionStatus.Closed;
      _NHibernaetSession.SaveOrUpdate(payerProfitSession);

      _NHibernaetSession.SaveOrUpdate(tradingSession);

      return tradingSession;
    }

    [TestMethod]
    public void CreateTradingSession_State_Open_Test()
    {
      D_TradingSession d_tradingSession = CreateTradingSession_State_Open();

      TransactionCommit();
    }

    [TestMethod]
    public void ChangeState_To_NeedEnsureProfibility_Test()
    {
      D_TradingSession d_tradingSession = CreateTradingSession_State_Open();

      d_tradingSession = ChangeState_To_NeedEnsureProfibility(d_tradingSession);

      System.Diagnostics.Trace.WriteLine(String.Format("Логин покупателя: {0}", d_tradingSession.BuyingMyCryptRequest.Buyer.Login));
      System.Diagnostics.Trace.WriteLine(String.Format("Логин продавца: {0}", d_tradingSession.BuyingMyCryptRequest.SellerUser.Login));

      TransactionCommit();
    }

    [TestMethod]
    public void ChangeState_To_WaitForProgressStart()
    {
      D_TradingSession d_tradingSession = CreateTradingSession_State_Open();

      d_tradingSession = ChangeState_To_NeedEnsureProfibility(d_tradingSession);

      d_tradingSession = ChangeState_To_WaitForProgressStart(d_tradingSession);

      TransactionCommit();
    }

    [TestMethod]
    public void ChangeState_To_SessionInProgress_Test()
    {
      D_TradingSession d_tradingSession = CreateTradingSession_State_Open();

      d_tradingSession = ChangeState_To_NeedEnsureProfibility(d_tradingSession);

      d_tradingSession = ChangeState_To_WaitForProgressStart(d_tradingSession);

      d_tradingSession = ChangeState_To_SessionInProgress(d_tradingSession);

      TransactionCommit();
    }

    [TestMethod]
    public void ChangeState_To_NeedProfit_Test()
    {
      D_TradingSession d_tradingSession = CreateTradingSession_State_Open();

      d_tradingSession = ChangeState_To_NeedEnsureProfibility(d_tradingSession);

      d_tradingSession = ChangeState_To_WaitForProgressStart(d_tradingSession);

      d_tradingSession = ChangeState_To_SessionInProgress(d_tradingSession);

      d_tradingSession = ChangeState_To_NeedProfit(d_tradingSession);

      TransactionCommit();
    }

    [TestMethod]
    public void ChangeState_To_ProfitConfirmation_Test()
    {
      D_TradingSession d_tradingSession = CreateTradingSession_State_Open();

      d_tradingSession = ChangeState_To_NeedEnsureProfibility(d_tradingSession);

      d_tradingSession = ChangeState_To_WaitForProgressStart(d_tradingSession);

      d_tradingSession = ChangeState_To_SessionInProgress(d_tradingSession);

      d_tradingSession = ChangeState_To_NeedProfit(d_tradingSession);

      d_tradingSession = ChangeState_To_ProfitConfirmation(d_tradingSession);

      TransactionCommit();
    }

    [TestMethod]
    public void Emulate_Trade_Operation_Wuth_Many_TS()
    {
      List<D_TradingSession> needProfitTSs = new List<D_TradingSession>();
      List<D_TradingSession> needEnsureTSs = new List<D_TradingSession>();

      for (ushort index = 1; index <= 20; index++)
      {
        D_TradingSession d_tradingSession = CreateTradingSession_State_Open();

        d_tradingSession = ChangeState_To_NeedEnsureProfibility(d_tradingSession);

        d_tradingSession = ChangeState_To_WaitForProgressStart(d_tradingSession);

        d_tradingSession = ChangeState_To_SessionInProgress(d_tradingSession);

        d_tradingSession = ChangeState_To_NeedProfit(d_tradingSession);

        needProfitTSs.Add(d_tradingSession);
      }

      _NHibernaetSession.Transaction.Commit();
      _NHibernaetSession.BeginTransaction();

      for(ushort index = 1; index <= 15; index++)
      {
        D_TradingSession d_tradingSession = CreateTradingSession_State_Open();

        d_tradingSession = ChangeState_To_NeedEnsureProfibility(d_tradingSession);

        needEnsureTSs.Add(d_tradingSession);
      }

      TransactionCommit();
    }

    [TestMethod]
    public void Delete_All_Session()
    {
      List<D_TradingSession> tradingSessions = _NHibernaetSession.Query<D_TradingSession>().ToList();

      foreach (var session in tradingSessions)
      {
        session.YieldSessionBills.ForEach(x => { x.AcceptorTradingSession = null; x.PayerTradingSession = null; _NHibernaetSession.SaveOrUpdate(x); });
        session.YieldSessionBills.Clear();

        _NHibernaetSession.Save(session);
        _NHibernaetSession.Transaction.Commit();
        _NHibernaetSession.BeginTransaction();

        _NHibernaetSession.Delete(session);
      }

      TransactionCommit();
    }


    #region Тесты для быстроко просмотра и манипуляции данными
    [TestMethod]
    public void Watch_User_Trading_Session()
    {
      D_TradingSession tradinSession = _NHibernaetSession.Query<D_TradingSession>().Where(t => t.BuyingMyCryptRequest.Buyer.Login == "Eagle").OrderByDescending(x => x.CreationDateTime).FirstOrDefault();

      Assert.IsTrue(tradinSession != null);
    }
    #endregion
  }
}
