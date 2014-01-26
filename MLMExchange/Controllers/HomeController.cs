using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MLMExchange.Helpers;

namespace MLMExchange.Controllers
{
  public class HomeController : Controller
  {
    [Auth]
    public ActionResult Index()
    {
      return View();
    }
  }
}
