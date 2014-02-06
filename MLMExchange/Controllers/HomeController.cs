using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MLMExchange.Lib;

namespace MLMExchange.Controllers
{
  public class HomeController : BaseController
  {
    [Auth]
    public ActionResult Index()
    {
      return View();
    }
  }
}
