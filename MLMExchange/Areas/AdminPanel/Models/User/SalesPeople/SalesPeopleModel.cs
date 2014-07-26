using Logic;
using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using NHibernate.Linq;
using System.Linq;

namespace MLMExchange.Areas.AdminPanel.Models.User.SalesPeople
{
  /// <summary>
  /// Модель для отображения продавцов
  /// </summary>
  public class SalesPeopleModel : AbstractModel
  {
    /// <summary>
    /// Продавцы, у которых можно купить my-crypt
    /// </summary>
    public List<BiddingParticipateApplicationModel> ActiveSales { get; set; }
    /// <summary>
    /// Запрещено ли текущему пользователю покупать MC.
    /// ИСпользовать аккуратно. Прямое обращение к базе без кэширования.
    /// </summary>
    public bool IsCurrentUserBuyMCDisabled
    {
      get
      {
        Func<D_TradingSession, bool> checkTSForDisabled = (D_TradingSession session) =>
        {
          if (session == null)
            return false;

          // Если покупатель и продавец одно лицо
          if (session.BuyingMyCryptRequest.SellerUser.Id == session.BuyingMyCryptRequest.Buyer.Id)
            return true;

          // Если у продавца уже закрылась заявка на продажу
          if (session.BuyingMyCryptRequest.SellerUser.Id == MLMExchange.Lib.CurrentSession.Default.CurrentUser.Id
            && session.BiddingParticipateApplication.State == BiddingParticipateApplicationState.Closed)
          {
            return false;
          }

          return true;
        };

        IEnumerable<D_TradingSession> openTradingSessions = _NhibernateSession.Query<D_TradingSession>()
          .Where(x => x.State != TradingSessionStatus.Closed && (x.BuyingMyCryptRequest.Buyer.Id == MLMExchange.Lib.CurrentSession.Default.CurrentUser.Id || x.BuyingMyCryptRequest.SellerUser.Id == MLMExchange.Lib.CurrentSession.Default.CurrentUser.Id));

        foreach(var session in openTradingSessions)
        {
          if (checkTSForDisabled(session))
            return true;
        }

        return false;
      }
    }

    /// <summary>
    /// Заявки, на которые данный пользователь откликнулся
    /// </summary>
    public List<BuyingMyCryptRequestModel> HistoryApplication { get; set; }
  }
}
