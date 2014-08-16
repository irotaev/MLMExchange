using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

namespace Logic
{
  public class RoleTypeAccessLevel : AbstractLogicObject<D_RoleTypeAccessLevel>
  {
    public RoleTypeAccessLevel(D_RoleTypeAccessLevel d_object)
      : base(d_object)
    {
    }

    public static explicit operator RoleTypeAccessLevel(D_RoleTypeAccessLevel dataBaseObject)
    {
      return new RoleTypeAccessLevel(dataBaseObject);
    }

    /// <summary>
    /// Попытаться изменить уровень доступа для данной роли
    /// </summary>
    /// <param name="flag">False - запретить доступ к торговым операциям, false - разрешить</param>
    /// <returns>Успешность операции</returns>
    public bool TryChangeAccessLevel(bool flag)
    {
      if (LogicObject.IsTradeEnable == flag)
        return true;

      LogicObject.IsTradeEnable = flag;

      return true;
    }

    /// <summary>
    /// Получить все уровни доступа для ролей
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<D_RoleTypeAccessLevel> GetAllRoleTypeAccessLevels()
    {
      return Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_RoleTypeAccessLevel>().ToList();
    }

    public static bool IsUserHasAccessToTradeSystem(D_User user)
    {
      var roleAccessLevels = GetAllRoleTypeAccessLevels();

      {
        var administratorRole = ((User)user).GetRole<D_AdministratorRole>();

        if (administratorRole != null)
          if (roleAccessLevels.Where(x => x.RoleType == RoleType.Administrator && x.IsTradeEnable).Count() == 1)
            return true;
      }

      {
        var userRole = ((User)user).GetRole<D_UserRole>();

        if (userRole != null)
          if (roleAccessLevels.Where(x => x.RoleType == RoleType.User && x.IsTradeEnable).Count() == 1)
            return true;
      }

      {
        var brokerRole = ((User)user).GetRole<D_BrokerRole>();

        if (brokerRole != null)
          if (roleAccessLevels.Where(x => x.RoleType == RoleType.Broker && x.IsTradeEnable).Count() == 1)
            return true;
      }

      {
        var leaderRole = ((User)user).GetRole<D_LeaderRole>();

        if (leaderRole != null)
          if (roleAccessLevels.Where(x => x.RoleType == RoleType.Leader && x.IsTradeEnable).Count() == 1)
            return true;
      }

      {
        var testerRole = ((User)user).GetRole<D_TesterRole>();

        if (testerRole != null)
          if (roleAccessLevels.Where(x => x.RoleType == RoleType.Tester && x.IsTradeEnable).Count() == 1)
            return true;
      }

      return false;
    }
  }
}
