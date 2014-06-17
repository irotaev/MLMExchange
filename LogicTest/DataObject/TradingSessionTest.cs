using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace LogicTest.DataObject
{
  [TestClass]
  public class TradingSessionPerformDataTest : AbstractPreformDataTest
  {
    /// <summary>
    /// Добавление платежей прибыли торговой сессии конкретному пользователю
    /// </summary>
    [TestMethod]
    public void AddNeedProfitBillTest()
    {
      D_User d_Buyer = _Session.Query<D_User>().Where(x => x.Login == "irotaev").FirstOrDefault();
      D_User d_Payer = _Session.Query<D_User>().Where(x => x.Login == "newbik").FirstOrDefault();

      if (d_Buyer == null || d_Payer == null)
        Assert.Fail();

      D_TradingSession d_acceptorTradingSession = _Session.Query<D_TradingSession>().Where(x => x.BuyingMyCryptRequest.Buyer.Id == d_Buyer.Id).FirstOrDefault();
      D_TradingSession d_payerTradingSession = _Session.Query<D_TradingSession>().Where(x => x.BuyingMyCryptRequest.Buyer.Id == d_Payer.Id).FirstOrDefault();

      if (d_acceptorTradingSession == null || d_acceptorTradingSession.State != TradingSessionStatus.NeedProfit)
        Assert.Fail();

      D_YieldSessionBill d_bill = new D_YieldSessionBill
      {
        PaymentState = BillPaymentState.WaitingPayment,
        Payer = d_Payer,
        PaymentAcceptor = d_Buyer,
        MoneyAmount = ((TradingSession)d_acceptorTradingSession).CalculateBuyerProfit(),
        PayerTradingSession = d_payerTradingSession,
        AcceptorTradingSession = d_acceptorTradingSession
      };

      _Session.SaveOrUpdate(d_bill);
      TransactionCommit();

      Assert.IsTrue(true);
    }
  }
}
