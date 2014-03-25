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

          if (loginUser != null)
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
              User user = new User
              {
                Login = userModel.Login,
                PasswordHash = Md5Hasher.ConvertStringToHash(userModel.Password),
                Email = userModel.Email,
                Name = userModel.Name,
                Surname = userModel.Surname,
                Patronymic = userModel.Patronymic,
              };

              #region Сохраняю фото
              if (userModel.Photo != null)
              {
                string fileName = String.Format("{0}_{1}", Guid.NewGuid().ToString(), System.IO.Path.GetExtension(userModel.Photo.FileName));
                string filePath = System.IO.Path.Combine(Server.MapPath("~/Uploads"), fileName);
                userModel.Photo.SaveAs(filePath);

                user.PhotoRelativePath = System.IO.Path.GetFileName(filePath);
              }
              #endregion

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
