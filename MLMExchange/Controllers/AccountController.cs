using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logic;
using MLMExchange.Models;
using MLMExchange.Models.Registration;

namespace MLMExchange.Controllers
{
  public class AccountController : Controller
  {
    public ActionResult Login(LoginModel loginModel)
    {
      return View(loginModel);
    }

    public ActionResult Register(UserModel userModel)
    {
      if (ModelState.IsValid)
      {
        
        using (var session = NHibernateConfiguration.Session.OpenSession())
        {
          using (var transaction = session.BeginTransaction())
          {
            User user = new User { Login = userModel.Login, PasswordHash = userModel.PasswordHash };

            session.Save(user);
            transaction.Commit();
          }
        }
      }

      return View(userModel);
    }
  }
}
