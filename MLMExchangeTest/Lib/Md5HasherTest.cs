using Logic;
using LogicTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLMExchange.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace MLMExchangeTest.Lib
{
  [TestClass]
  public class Md5HasherTest : AbstractTest
  {
    #region Просмотр и манипуляция с данными
    [TestMethod]
    public void Change_Password()
    {
      const string newPassword = "be2eb1834140d7e9a0761c0b60dbf85e";

      D_User user = _NHibernaetSession.Query<D_User>().Where(x => x.Login == "t_andrey").FirstOrDefault();

      Assert.IsTrue(user != null, "Такого пользователя не существует");

      user.PasswordHash = Md5Hasher.ConvertStringToHash(newPassword);

      _NHibernaetSession.SaveOrUpdate(user);

      TransactionCommit();
    }
    #endregion
  }
}
