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
    /// Создать тестового пользователя
    /// </summary>
    /// <param name="roles">Роли пользователя</param>
    /// <param name="isSaveObject">Сохронять ли объект</param>
    /// <returns>Тестовый пользователь</returns>
    public static D_User CreateUser(IList<D_AbstractRole> roles, bool isSaveObject = true)
    {
      if (roles == null)
        throw new TestException("Не переданы роли. Невозможно создать пользователя");

      string postfix = Guid.NewGuid().ToString();

      D_User testUser = new D_User
      {
        Email = "test_email@email.com",
        Login = "TestUser_" + postfix,
        Name = "TestUser_Name_" + postfix,
        PasswordHash = _DefaultUserPassword,
        Patronymic = "TestUser_Patronymic_" + postfix,
        Surname = "TestUser_Surname_" + postfix
      };

      foreach (var role in roles)
      {
        if (role is D_UserRole)
        {
          ((D_UserRole)role).MyCryptCount = 100000;
        }

        role.User = testUser;
        testUser.Roles.Add(role);
      }

      #region PaymentSystemGroup
      testUser.PaymentSystemGroup = new D_PaymentSystemGroup { User = testUser };
      testUser.PaymentSystemGroup.BankPaymentSystems.Add(new D_BankPaymentSystem
      {
        BankName = "TestBankSystem_" + postfix,
        BIK = "123",
        CardNumber = "123",
        CurrentAccount = "123",
        INN = "123",
        IsDefault = true,
        KPP = "123",
        UserName = "System",
        UserPatronymic = "System",
        UserSurname = "System"
      });
      testUser.PaymentSystemGroup.ElectronicPaymentSystems.Add(new D_ElectronicPaymentSystem
      {
        ElectronicName = "TestElectoronicSystem_" + postfix,
        PurseNumber = "123",
        IsDefault = false
      });
      #endregion

      if (isSaveObject)
        _NHibernaetSession.SaveOrUpdate(testUser);

      return testUser;
    }

    /// <summary>
    /// Создать тестового пользователя.
    /// Пользователя создастся с единственной ролью пользователя. 
    /// </summary>
    /// <param name="isSaveObject">Сохронять ли объект</param>
    /// <returns>Тестовый пользователь</returns>
    public static D_User CreateUser(bool isSaveObject = true)
    {
      return CreateUser(new List<D_AbstractRole>() { new D_UserRole() }, isSaveObject);
    }

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
