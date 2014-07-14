using Logic.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
  /// <summary>
  /// Прокси-объект заявки на участие в торгах
  /// </summary>
  public class BiddingParticipateApplication : AbstractLogicObject<D_BiddingParticipateApplication>
  {
    public BiddingParticipateApplication(D_BiddingParticipateApplication dataObject) : base(dataObject) { }

    public static explicit operator BiddingParticipateApplication(D_BiddingParticipateApplication dataBaseObject)
    {
      return new BiddingParticipateApplication(dataBaseObject);
    }

    /// <summary>
    /// Списать mycrypt
    /// </summary>
    public void WriteOfBuyedMyCrypt()
    {
      if (LogicObject.TradingSession.BuyingMyCryptRequest == null)
        throw new Logic.Lib.ApplicationException("No buyer for this trading session");

      long sellerMyCryptTotalCount = ((User)LogicObject.Seller).GetRole<D_UserRole>().MyCryptCount;
      long tradingSessionBuingmyCryptCount = LogicObject.TradingSession.BuyingMyCryptRequest.MyCryptCount;

      if (sellerMyCryptTotalCount < tradingSessionBuingmyCryptCount)
      {
        throw new UserVisibleException(String.Format(Logic.Properties.PrivateResources.Exception__BiddingParticipateApplication_NotEnoughtMyCryptForWriteOut, 
          sellerMyCryptTotalCount, tradingSessionBuingmyCryptCount));
      }

      ((User)LogicObject.Seller).GetRole<D_UserRole>().MyCryptCount -= tradingSessionBuingmyCryptCount;
    }
  }
}
