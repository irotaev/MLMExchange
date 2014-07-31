using Logic.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicTest
{
  [TestClass]
  public class SsmsTest : AbstractTest
  {
    [TestMethod]
    public void SendMessage()
    {
      object request = new Ssms().SendMessage("Hellow world!", "79164289256");
    }

    [TestMethod]
    public void GenerateSmsCode()
    {
      string code = Ssms.GenerateSmsCode();

      Assert.IsTrue(!String.IsNullOrWhiteSpace(code) && code.Length == 6, "Неправильный формат кода");
    }
  }
}
