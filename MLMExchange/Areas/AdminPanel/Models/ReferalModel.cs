using MLMExchange.Lib;
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
  public class ReferalListModel : AbstractModel, ILogicDataGetter<Logic.User, ReferalListModel>
  {
    private Logic.User _user;

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

    public long? UserReferalId
    {
      get 
      {
        if (_user == null)
          throw new BindNotCallException<Logic.User>();

        Logic.D_UserRole userRole =  GetCurrentUser().UserRoles.FirstOrDefault(x => (x as Logic.D_UserRole) != null) as Logic.D_UserRole;

        if (userRole == null)
        {
          return null;
        }
        else
        {
          return userRole.Id;
        }
      }
    }

    /// <summary>
    /// Получить пользователя из логики
    /// </summary>
    /// <returns></returns>
    public UserModel GetCurrentUser()
    {
        if (_user == null)
          throw new BindNotCallException<Logic.User>();

        return new UserModel().Bind(_user.LogicObject);
    }

    public ReferalListModel Bind(Logic.User @object)
    {
      if (@object == null)
        throw new ArgumentNullException("@object");

      _user = @object;

      return this;
    }
  }
}