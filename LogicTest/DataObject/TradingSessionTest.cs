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
    public static D_TradingSession CreateTradingSession_State_Open()
    {
      D_User seller = D_UserTest.CreateUser();
      D_User buyer = D_UserTest.CreateUser(false);

      #region Назначаю рефералов
      buyer.RefererRole = ((User)seller).GetRole<D_UserRole>();

      _NHibernaetSession.SaveOrUpdate(buyer);
      #endregion

      _NHibernaetSession.SaveOrUpdate(seller);
      _NHibernaetSession.SaveOrUpdate(buyer);

      D_BiddingParticipateApplication participateApplication = BiddingParticipateApplicationTest.CreateBiddingParticipateApplication(seller, 2000);

      D_SystemSettings systemSettings = new D_SystemSettings
      {
        CheckPaymentPercent = 5,
        MaxMyCryptCount = 10000,
        ProfitPercent = 10,
        Quote = 10,
        TradingSessionDuration = 0.1m
      };

      _NHibernaetSession.SaveOrUpdate(systemSettings);

      BuyingMyCryptRequest buyingRequest = BuyingMyCryptRequestTest.CreateBuyingMyCryptRequest(buyer, participateApplication, systemSettings, 1000);      

      D_TradingSession d_tradingSession = TradingSession.OpenTradingSession(buyingRequest);

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
      acceptedTradingSession.State = TradingSessionStatus.NeedProfit;

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

    public static D_TradingSession ChangeState_To_ProfitConfirmation(D_TradingSession tradingSession)
    {
      if (tradingSession == null)
        throw new TestParametrNullException("tradingSession");

      if (tradingSession.State != TradingSessionStatus.SessionInProgress)
        throw new TestException("Торговая сессия может быть переведена в состояние 'NeedProfit и ProfitConfirmation' только из состояния 'SessionInProgress'");

      System.Threading.Thread.Sleep((int)(tradingSession.SystemSettings.TradingSessionDuration * 60 * 1000));

      ((TradingSession)tradingSession).TryChangeStatus(TradingSessionStatus.NeedProfit);      

      D_TradingSession payerProfitSession = TradingSessionTest.CreateTradingSession_State_Open();

      #region Создание платежей на прибыль текущей сессии
      decimal buyerProfit = tradingSession.BuyingMyCryptRequest.MyCryptCount + ((TradingSession)tradingSession).CalculateBuyerProfit();
      decimal modulor = buyerProfit % 2;
      decimal divisionResult = buyerProfit / 2;

      for (uint index = 1; index <= 3; index++)
      {
        if (index == 3 && modulor == 0)
          continue;

        D_YieldSessionBill yieldSessionBill = new D_YieldSessionBill
        {
          AcceptorTradingSession = tradingSession,
          MoneyAmount = index == 3 ? modulor : divisionResult,
          Payer = payerProfitSession.BuyingMyCryptRequest.Buyer,
          PayerTradingSession = payerProfitSession,
          PaymentAcceptor = tradingSession.BuyingMyCryptRequest.Buyer,
          PaymentState = BillPaymentState.EnoughMoney
        };

        ((Bill)yieldSessionBill).AddPayment(new Payment
        {
          Bill = yieldSessionBill,
          PaymentSystem = payerProfitSession.BuyingMyCryptRequest.Buyer.PaymentSystemGroup.BankPaymentSystems.FirstOrDefault(),
          RealMoneyAmount = index == 3 ? modulor : divisionResult,
          Payer = payerProfitSession.BuyingMyCryptRequest.Buyer
        });

        _NHibernaetSession.SaveOrUpdate(yieldSessionBill);
      }
      #endregion

      ((TradingSession)tradingSession).TryChangeStatus(TradingSessionStatus.ProfitConfirmation);

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
    public void ChangeState_To_ProfitConfirmation_Test()
    {
      D_TradingSession d_tradingSession = CreateTradingSession_State_Open();

      d_tradingSession = ChangeState_To_NeedEnsureProfibility(d_tradingSession);

      d_tradingSession = ChangeState_To_WaitForProgressStart(d_tradingSession);

      d_tradingSession = ChangeState_To_SessionInProgress(d_tradingSession);

      d_tradingSession = ChangeState_To_ProfitConfirmation(d_tradingSession);

      TransactionCommit();
    }
  }
}
