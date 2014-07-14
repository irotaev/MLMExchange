using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public static explicit operator User(D_User dataUser)
    {
      return new User(dataUser);
    }
  }
}
