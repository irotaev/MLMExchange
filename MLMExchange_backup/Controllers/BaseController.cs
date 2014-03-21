using Logic;
using MLMExchange.Models.Registration;
using MLMExchange.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MLMExchange.Controllers
{
  public abstract class BaseController : Controller
  {
    public CurrentSession CurrentSession { get; private set; }

    protected override void Initialize(System.Web.Routing.RequestContext requestContext)
    {
      base.Initialize(requestContext);

      var authorized = requestContext.HttpContext.Request.Cookies["_AUTHORIZE"];

      if (authorized != null && Boolean.Parse(authorized.Value) == true)
      {
        CurrentSession = new CurrentSession(Session);
      }

      #region Модель для логина
      LoginModel loginModel = new LoginModel();

      TryUpdateModel<LoginModel>(loginModel);

      ViewBag.LoginModel = loginModel;
      #endregion
    }
  }
}
