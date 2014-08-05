using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicTest.DataObject
{
  [TestClass]
  public class BuyingMyCryptRequestTest : AbstractTest
  {
    /// <summary>
    /// Создать запрос на покупку my crypt.
    /// </summary>
    /// <param name="buyer">Покупатель</param>
    /// <param name="participateApplication">Заявка на продажу MC</param>
    /// <param name="systemSettings">Системные настройки</param>
    /// <param name="myCryptCount">Количество MC для покупки</param>
    /// <returns>Запрос на покупку MC</returns>
    public static BuyingMyCryptRequest CreateBuyingMyCryptRequest(D_User buyer, D_BiddingParticipateApplication participateApplication, D_SystemSettings systemSettings, long myCryptCount)
    {
      string postfix = Guid.NewGuid().ToString();

      BuyingMyCryptRequest request = new BuyingMyCryptRequest
      {
        Buyer = buyer,
        Comment = "Коммент_" + postfix,
        MyCryptCount = myCryptCount,
        SellerUser = participateApplication.Seller,
        BiddingParticipateApplication = participateApplication,
        SystemSettings = systemSettings
      };

      return request;
    }

    [TestMethod]
    public void CreateBuyingRequest()
    {
      D_User buyer = D_UserTest.CreateUser();
      D_User seller = D_UserTest.CreateUser();

      D_BiddingParticipateApplication biddingApplication = BiddingParticipateApplicationTest.CreateBiddingParticipateApplication(seller, 1200);

      BuyingMyCryptRequest request = CreateBuyingMyCryptRequest(
        buyer,
        biddingApplication,
         new D_SystemSettings
          {
            CheckPaymentPercent = 5,
            MaxMyCryptCount = 2000,
            ProfitPercent = 7,
            Quote = 10,
            TradingSessionDuration = 0.1m
          },
        800);

      _NHibernaetSession.Save(request);

      TransactionCommit();

      Assert.IsTrue(request != null, "Заявка не была создана");
    }
  }
}
