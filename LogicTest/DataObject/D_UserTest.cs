using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;

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
          PasswordHash = "21232f297a57a5a743894a0e4a801fc3", // Пароль: admin
          Name = "TestUser",
          PaymentSystemGroup = new PaymentSystemGroup()
        };

      administratorUser.Roles.Add(new D_AdministratorRole { User = administratorUser });

      _Session.SaveOrUpdate(administratorUser);

      _Session.Transaction.Commit();

      D_AbstractRole findRole = _Session.Query<D_AbstractRole>().Where(x => x.User.Login == login).FirstOrDefault();

      Assert.IsTrue(findRole != null);

      TransactionCommit(true);
    }
  }
}
