using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Areas.AdminPanel.Controllers.PaymentSystems
{
  [Auth]
  public class BillController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Browse()
        {
          return null;
        }

    }
}
