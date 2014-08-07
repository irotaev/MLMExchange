using Logic.Lib.PaymentSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicTest
{
  [TestClass]
  public class PerfectMoneyTest
  {
    [TestMethod]
    public void Verifire()
    {
      PerfectMoney perfectMoney = new PerfectMoney();
      var result = perfectMoney.Verifire("9459667", "b0d406a92dfbe07cd820675518668c5f", "U5305686", "U5305686", 0, 1, 22222);

      Assert.IsTrue(result != null, "Ответ от сервера не получен");
    }

    [TestMethod]
    public void Check_Hash_Generator()
    {
      string hash = PerfectMoney.GenerateV2Hash("A-554d", "U5305686", "5.12", "USD", "6577", "434343", "85003133456");

      Assert.AreEqual(hash, "047F54EE13797E15DFDA5D679B70BEFE", "Неправильно посчитан hash. Не совпадает с тем, что на сайте: https://www.perfectmoney.is/acct/md5check.html");
    }
  }
}
