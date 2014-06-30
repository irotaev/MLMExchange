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
using Microsoft.Practices.Unity;

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
        D_User loginUser = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.QueryOver<D_User>().Where(x => x.Login == loginModel.Login).List().FirstOrDefault();

        if (loginUser != null && loginUser.PasswordHash == Md5Hasher.ConvertStringToHash(loginModel.Password))
        {
          var authCook = new HttpCookie("_AUTHORIZE", "true");
          authCook.Expires = DateTime.Now.AddYears(1);
          HttpContext.Response.Cookies.Add(authCook);
          Session.Add("Login", loginUser.Login);
          Session.Add("Authorized", true);
        }
      }

      return Redirect("/AdminPanel/User/ControlPanel");
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
          #region Сохраняю фото
          if (userModel.Photo != null)
          {
            string filePath = MLMExchange.Lib.Image.Image.SaveImage(userModel.Photo, Server);

            userModel.PhotoRelativePath = filePath;
          }
          #endregion

          D_User user = userModel.UnBind();

          user.Roles.Add(new D_UserRole { User = user });

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(user);
        }
      }

      return View(userModel);
    }
  }
}
