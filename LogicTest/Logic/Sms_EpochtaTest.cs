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
  public class Sms_EpochtaTest : AbstractTest
  {
    [TestMethod]
    public void Send_Single_Sms()
    {
      Sms_Epochta sms = new Sms_Epochta("79164289256");
      sms.SendMessage("Hellow world! Привет");
    }
  }
}
