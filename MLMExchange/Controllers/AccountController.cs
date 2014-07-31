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
using Logic.Lib;

namespace MLMExchange.Controllers
{
  public class AccountController : BaseController
  {
    private bool TryLogin(string login, string passwordHash)
    {
      bool result = false;

      D_User loginUser = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.QueryOver<D_User>().Where(x => x.Login == login).List().FirstOrDefault();

      if (loginUser != null && loginUser.PasswordHash == passwordHash)
      {
        var authCook = new HttpCookie("_AUTHORIZE", "true");
        authCook.Expires = DateTime.Now.AddYears(1);
        HttpContext.Response.Cookies.Add(authCook);
        Session.Add("Login", loginUser.Login);
        Session.Add("Authorized", true);

        result = true;
      }

      return result;
    }

    [HttpPost]
    public ActionResult Login(string redirectUrl = null)
    {
      ModelState.Clear();

      LoginModel loginModel = new LoginModel();

      TryUpdateModel<LoginModel>(loginModel);

      if (ModelState.IsValid)
      {
        if (!TryLogin(loginModel.Login, Md5Hasher.ConvertStringToHash(loginModel.Password)))
        {
          throw new Logic.Lib.UserVisibleException(String.Format("{0}{1}", MLMExchange.Properties.ResourcesA.Exception_LoginFaield, "!"));
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
          userModel.Validate(this.ModelState);

          if (this.ModelState.IsValid)
          {
            user.Roles.Add(new D_UserRole { User = user });

            #region Отправка смс
#if DEBUG
            user.ConfirmationCode = "tester";
#else 
            user.ConfirmationCode = Ssms.GenerateSmsCode();
            new Ssms().SendMessage(String.Format(MLMExchange.Properties.PrivateResource.AccountRegister__Sms_ActivationCode, user.ConfirmationCode), user.PhoneNumber);
#endif
            #endregion

            Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(user);

            TryLogin(user.Login, user.PasswordHash);

            return Redirect("/Account/Confirm");
          }
        }
      }

      return View(userModel);
    }

    [Auth]
    public ActionResult Confirm(ConfirmModel model)
    {
      ModelState.Clear();

      if (CurrentSession.Default.CurrentUser == null)
        throw new MLMExchange.Lib.Exception.UserVisible__CurrentActionAccessDenied();

      if (CurrentSession.Default.CurrentUser.IsUserRegistrationConfirm)
        throw new UserVisibleException(MLMExchange.Properties.PrivateResource.AccountConfirm__Exception_YourAccountAlreadyConfirmed);

      model.PhoneNumber = CurrentSession.Default.CurrentUser.PhoneNumber;

      if (ControllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        TryUpdateModel<ConfirmModel>(model);

        if (ModelState.IsValid)
        {
          if (model.ConfirmationCode == CurrentSession.Default.CurrentUser.ConfirmationCode)
          {
            CurrentSession.Default.CurrentUser.IsUserRegistrationConfirm = true;

            _NHibernateSession.SaveOrUpdate(CurrentSession.Default.CurrentUser);

            return Redirect("/Account/Success");
          }
          else
          {
            ModelState.AddModelError("ConfirmationCode", MLMExchange.Properties.ResourcesA.ConfirmationCodeIncorrect);
          }
        }
      }

      return View(model);
    }

    public ActionResult Success()
    {
      return View();
    }
  }
}
