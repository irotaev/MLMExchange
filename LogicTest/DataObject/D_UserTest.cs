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
        Email = postfix.Substring(0, 20) + "@email.com",
        Login = "TestUser_" + postfix,
        Name = "TestUser_Name_" + postfix,
        PasswordHash = _DefaultUserPassword,
        Patronymic = "TestUser_Patronymic_" + postfix,
        Surname = "TestUser_Surname_" + postfix,
        PhoneNumber = "+" + postfix.Substring(0, 10),
        Skype = "Skype__" + postfix.Substring(0, 10),
        ConfirmationCode = postfix.Substring(0, 6),
        IsUserRegistrationConfirm = true
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
      D_User adminUser = CreateUser(new List<D_AbstractRole> { new D_AdministratorRole() });

      Assert.IsTrue(adminUser != null, "Пользователь не создан");

      TransactionCommit();
    }

    [TestMethod]
    public void Create_Many_Users()
    {
      const uint maxUserCount = 300;

      for(uint index = 1; index < maxUserCount; index++)
      {
        D_User d_user = CreateUser();
      }

      TransactionCommit();
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

    /// <summary>
    /// Удаление пользователя из базы
    /// </summary>
    [TestMethod]
    public void Delete_User()
    {
      // Логин пользователя, которого удаляю
      const string userLogin = "jimmy";

      D_User d_user = _NHibernaetSession.Query<D_User>().Where(x => x.Login == userLogin).FirstOrDefault();

      if (d_user == null)
        Assert.Fail("Такого пользователя не существует в базе");

      _NHibernaetSession.Delete(d_user);

      TransactionCommit();
    }

    [TestMethod]
    public void Delete_User_By_PhoneNumber()
    {
      // Телефон пользователя, которого удаляю
      const string userPhoneNumber = "79164289256";
        
      D_User d_user = _NHibernaetSession.Query<D_User>().Where(x => x.PhoneNumber == userPhoneNumber).FirstOrDefault();

      if (d_user == null)
        Assert.Fail("Такого пользователя не существует в базе");

      _NHibernaetSession.Delete(d_user);

      TransactionCommit();
    }
  }
}
