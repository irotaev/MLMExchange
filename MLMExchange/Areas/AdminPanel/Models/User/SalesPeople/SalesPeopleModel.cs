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
        D_TradingSession openTradingSession = _NhibernateSession.Query<D_TradingSession>()
          .Where(x => x.State != TradingSessionStatus.Closed && (x.BuyingMyCryptRequest.Buyer.Id == MLMExchange.Lib.CurrentSession.Default.CurrentUser.Id
              || x.BuyingMyCryptRequest.SellerUser.Id == MLMExchange.Lib.CurrentSession.Default.CurrentUser.Id)).FirstOrDefault();

        if (openTradingSession == null)
          return false;

        // Если покупатель и продавец одно лицо
        if (openTradingSession.BuyingMyCryptRequest.SellerUser.Id == openTradingSession.BuyingMyCryptRequest.Buyer.Id)
          return true;

        // Если у продавца уже закрылась заявка на продажу
        if (openTradingSession.BuyingMyCryptRequest.SellerUser.Id == MLMExchange.Lib.CurrentSession.Default.CurrentUser.Id
          && openTradingSession.BiddingParticipateApplication.State == BiddingParticipateApplicationState.Closed)
        {
          return false;
        }

        return true;
      }
    }

    /// <summary>
    /// Заявки, на которые данный пользователь откликнулся
    /// </summary>
    public List<BuyingMyCryptRequestModel> HistoryApplication { get; set; }
  }
}
