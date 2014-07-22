using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Models
{
  /// <summary>
  /// Модель реферала
  /// </summary>
  public class ReferalModel : AbstractModel
  {
    /// <summary>
    /// Реферал
    /// </summary>
    public UserModel Referal { get; set; }
    /// <summary>
    /// Общая прибыль рефереру
    /// </summary>
    public decimal RefererTotalrofit { get; set; }
  }

  /// <summary>
  /// Модель списка рефералов
  /// </summary>
  public class ReferalListModel : AbstractModel
  {
    public ReferalListModel()
    {
      Referals = new List<ReferalModel>();
    }

    public List<ReferalModel> Referals { get; set; }
    /// <summary>
    /// Общая прибыль по всем рефералам
    /// </summary>
    public decimal RefererTotalProfit
    {
      get
      {
        return Referals.Sum(x => x.RefererTotalrofit);
      }
    }
  }
}