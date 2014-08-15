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
    /// Получить все уровни доступа для ролей
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<D_RoleTypeAccessLevel> GetAllRoleTypeAccessLevels()
    {
      return Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_RoleTypeAccessLevel>().ToList();
    }
  }
}
