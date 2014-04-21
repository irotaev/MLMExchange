using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logic;
using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System.Security.Cryptography;
using MLMExchange.Lib;
using NHibernate.Linq;

namespace MLMExchange.Controllers
{
  public class AccountController : BaseController
  {
    [HttpPost]
    public ActionResult Login(string redirectUrl = null)
    {
      ModelState.Clear();

      LoginModel loginModel = new LoginModel();

      TryUpdateModel<LoginModel>(loginModel);

      if (ModelState.IsValid)
      {
        using (var session = NHibernateConfiguration.Session.OpenSession())
        {
          User loginUser = session.QueryOver<User>().Where(x => x.Login == loginModel.Login).List().FirstOrDefault();

          if (loginUser != null && loginUser.PasswordHash == Md5Hasher.ConvertStringToHash(loginModel.Password))
          {
            var authCook = new HttpCookie("_AUTHORIZE", "true");
            authCook.Expires = DateTime.Now.AddYears(1);
            HttpContext.Response.Cookies.Add(authCook);
            Session.Add("Login", loginUser.Login);
            Session.Add("Authorized", true);
          }
        }
      }

      return Redirect(Request.UrlReferrer.ToString());
    }

    public ActionResult LogOut()
    {
      Session.Clear();
      return Redirect(Request.UrlReferrer.ToString());
    }

    public ActionResult Register(UserModel userModel)
    {
      ModelState.Clear();

      if (ControllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        TryUpdateModel<UserModel>(userModel);

        if (ModelState.IsValid)
        {
          using (var session = NHibernateConfiguration.Session.OpenSession())
          {
            using (var transaction = session.BeginTransaction())
            {
              #region Сохраняю фото
              if (userModel.Photo != null)
              {
                string filePath = MLMExchange.Lib.Image.Image.SaveImage(userModel.Photo, Server);

                userModel.PhotoRelativePath = filePath;
              }
              #endregion

              User user = UserModel.UnBind(userModel);

              session.Save(user);
              transaction.Commit();
            }
          }
        }
      }

      return View(userModel);
    }
  }
}
