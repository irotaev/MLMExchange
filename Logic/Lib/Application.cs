using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

namespace Logic.Lib
{
  /// <summary>
  /// Приложение
  /// </summary>
  public sealed class Application
  {
    static Application()
    {
      AddAdministratorRoleUsers();
    }

    /// <summary>
    /// Добавить пользователей с ролью администратора в проект
    /// </summary>
    private static void AddAdministratorRoleUsers()
    {
      D_User adminUser = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_User>().Where(x => x.Login == "administrator_irotaev").FirstOrDefault();

      if (adminUser == null)
      {
        adminUser = new D_User
        {
          Login = "administrator_irotaev",
          PasswordHash = "25d55ad283aa400af464c76d713c07ad", // Пароль: 12345678,
          PaymentSystemGroup = new PaymentSystemGroup()
        };

        adminUser.Roles = new List<D_AbstractRole> { new D_AdministratorRole { User = adminUser } };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(adminUser);
      }
    }
  }
}
