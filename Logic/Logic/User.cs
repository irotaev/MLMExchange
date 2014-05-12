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

    public static explicit operator User(D_User dataUser)
    {
      return new User(dataUser);
    }
  }
}
