using MLMExchange.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  public class UserController : BaseController
  {
    public ActionResult Info()
    {
      return View();
    }
  }
}