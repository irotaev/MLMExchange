using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Models.User
{
  public class ControlPanelModel : BaseModel
  {
    /// <summary>
    /// Состояние заявки на продажу my-crypt
    /// </summary>
    public BiddingParticipateApplicationStateModel BiddingParticipateApplicationStateModel { get; set; }

    /// <summary>
    /// Модель торговой сессии
    /// </summary>
    public TradingSessionModel TradingSessionModel { get; set; }
  }
}