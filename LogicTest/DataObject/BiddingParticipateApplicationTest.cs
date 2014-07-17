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
  public class BiddingParticipateApplicationTest : AbstractTest
  {
    /// <summary>
    /// Создать заявку на продажу MC
    /// </summary>
    /// <param name="seller">Продавец</param>
    /// <param name="myCryptCount">Количество MC на продажу</param>
    /// <returns></returns>
    public static D_BiddingParticipateApplication CreateBiddingParticipateApplication(D_User seller, long myCryptCount)
    {
      D_BiddingParticipateApplication participateApplication = new D_BiddingParticipateApplication
      {
        MyCryptCount = myCryptCount,
        Seller = seller
      };

      //_NHibernaetSession.Save(participateApplication);

      return participateApplication;
    }
  }
}
