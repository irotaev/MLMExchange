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
  }
}
