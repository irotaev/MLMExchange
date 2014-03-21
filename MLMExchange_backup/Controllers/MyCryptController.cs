using MLMExchange.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Controllers
{
  [Auth]
  public class MyCryptController : BaseController
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}
