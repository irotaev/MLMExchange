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
  public class BillTest : AbstractTest
  {
    [TestMethod]
    public void DeleteAllBills()
    {
      List<D_Bill> bills = _NHibernaetSession.Query<D_Bill>().ToList();

      foreach (var bill in bills)
        _NHibernaetSession.Delete(bill);

      TransactionCommit();

      Assert.IsTrue(true);
    }
  }
}
