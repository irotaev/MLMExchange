using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using NHibernate;


namespace LogicTest.DataObject
{
  [TestClass]
  public class D_UserTest : AbstractTest
  {
    /// <summary>
    /// Создать пользователя с ролью администратора
    /// </summary>
    [TestMethod]
    public void CreateAdministratorRoleUser()
    {
      string login = "TestUser_" + Guid.NewGuid();

      D_User administratorUser = new D_User
        {
          Login = login,
          PasswordHash = "25d55ad283aa400af464c76d713c07ad", // Пароль: 12345678
          Name = "TestUser",
          PaymentSystemGroup = new D_PaymentSystemGroup()
        };

      administratorUser.Roles.Add(new D_AdministratorRole { User = administratorUser });

      _NHibernaetSession.SaveOrUpdate(administratorUser);

      _NHibernaetSession.Transaction.Commit();

      D_AbstractRole findRole = _NHibernaetSession.Query<D_AbstractRole>().Where(x => x.User.Login == login).FirstOrDefault();

      Assert.IsTrue(findRole != null);

      TransactionCommit(true);
    }

    /// <summary>
    /// Получить рандомного поьзователя
    /// </summary>
    [TestMethod]
    public void Get_Random_User()
    {
      D_User randomUser = _NHibernaetSession.QueryOver<D_User>().OrderByRandom().List().FirstOrDefault();

      Assert.IsTrue(randomUser != null, "Не нашли рандомного пользователя");
    }
  }
}
