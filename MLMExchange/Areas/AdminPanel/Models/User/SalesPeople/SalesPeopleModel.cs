using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    /// Заявки, на которые данный пользователь откликнулся
    /// </summary>
    public List<BuyingMyCryptRequestModel> HistoryApplication { get; set; }
  }
}
