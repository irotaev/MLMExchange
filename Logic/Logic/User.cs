using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

namespace Logic
{
  /// <summary>
  /// Прокси-класс пользователя
  /// </summary>
  public class User : AbstractLogicObject<D_User>
  {
    public User(D_User dataUser) : base(dataUser) { }

    /// <summary>
    /// Имеется ли у данного пользователя роль администратора
    /// </summary>
    /// <returns></returns>
    public bool IsAdministratorRoleExsists()
    {
      return LogicObject.Roles.Any(r => r.GetType() == typeof(D_AdministratorRole));
    }

    /// <summary>
    /// Получить роль пользователя по ее типу
    /// </summary>
    /// <typeparam name="TRole">Тип роли</typeparam>
    /// <returns>Роль пользователя. Null - если не найдена</returns>
    public TRole GetRole<TRole>() where TRole : D_AbstractRole
    {
      TRole role = LogicObject.Roles.Where(x => ((BaseObject)x).GetRealType() == typeof(TRole)).FirstOrDefault() as TRole;

      return role;
    }

    #region Операции с рефералами
    /// <summary>
    /// Попробовать заплатить прибыль рефереру для данного пользователя и
    /// переданной торговой сессиии.
    /// </summary>
    /// <param name="tradingSession">Торговая сессия, с которой берется прибыль</param>
    /// <returns>False - невозможно заплатить отчисления, true - отчисления выплачены</returns>
    public bool TryPayRefererProfit(D_TradingSession tradingSession)
    {
      if (tradingSession == null)
        return false;

      if (LogicObject.RefererRole == null)
        return false;

      User referer = (User)LogicObject.RefererRole.User;
      D_UserRole userRole = GetRole<D_UserRole>();

      if (userRole == null)
        return false;

      D_ReferalProfit refererProfit = new D_ReferalProfit
      {
        UserRole = userRole,
        TradingSession = tradingSession,
      };

      decimal tradingSessionProfit = ((TradingSession)tradingSession).CalculateBuyerProfit();

      if (referer.GetRole<D_AdministratorRole>() != null)
      {
        refererProfit.RefererProfit = tradingSessionProfit * 0.05m;
      }
      else if (referer.GetRole<D_LeaderRole>() != null)
      {
        refererProfit.RefererProfit = tradingSessionProfit * 0.04m;
      }
      else if (referer.GetRole<D_TesterRole>() != null)
      {
        refererProfit.RefererProfit = tradingSessionProfit * 0.03m;
      }
      else if (referer.GetRole<D_BrokerRole>() != null)
      {
        refererProfit.RefererProfit = tradingSessionProfit * 0.02m;
      }
      else if (referer.GetRole<D_UserRole>() != null)
      {
        refererProfit.RefererProfit = tradingSessionProfit * 0.01m;
      }
      else
      {
        return false;
      }

      _NHibernateSession.Save(refererProfit);

      return true;
    }

    /// <summary>
    /// Посчитать общий профит для реферера для данного пользователя
    /// </summary>
    /// <returns>Общий профит</returns>
    public decimal CalculateTotalReferalProfit()
    {
      decimal totalProfit = 0m;

      D_UserRole userRole = GetRole<D_UserRole>();

      if (userRole == null)
        return totalProfit;

      IEnumerable<D_User> referals = _NHibernateSession.Query<D_User>().Where(x => x.RefererRole.Id == userRole.Id);

      foreach(var referal in referals)
      {
        D_UserRole referalUserRole = ((User)referal).GetRole<D_UserRole>();

        if (referalUserRole == null)
          throw new ApplicationException("У реферала нет роли пользователя системы. Такого быть не должно.");

        totalProfit += ((UserRole)referalUserRole).CalculateTotalRefererProfit();
      }

      return totalProfit;
    }
    #endregion

    public static explicit operator User(D_User dataUser)
    {
      return new User(dataUser);
    }

    public static int GetUserCountToStart(ushort count)
    {
      int counter;
      int userCount = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_User>().Where(x => x.IsDisabled == false).Count();

      counter = count - userCount;

      if(counter <= 0)
        counter = 0;

      return counter;
    }
  }
}
