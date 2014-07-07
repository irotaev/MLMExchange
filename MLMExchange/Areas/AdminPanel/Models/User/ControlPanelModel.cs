using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Models.User
{
  /// <summary>
  /// Модель контрольной панели
  /// </summary>
  public class ControlPanelModel : AbstractModel
  {
    /// <summary>
    /// Состояние заявки на продажу my-crypt
    /// </summary>
    public BiddingParticipateApplicationStateModel BiddingParticipateApplicationStateModel { get; set; }

    /// <summary>
    /// Модель торговой сессии
    /// </summary>
    public TradingSessionModel TradingSessionModel { get; set; }

    /// <summary>
    /// Модель контрольной панели пользователя
    /// </summary>
    public UserControlBlockModel UserControlBlockModel { get; set; }
  }
}