using Logic;
using MLMExchange.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MLMExchange.Lib
{
  /// <summary>
  /// Аттрибут проверки авторизации
  /// </summary>
  public class AuthAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);

      BaseController baseController = filterContext.Controller as BaseController;

      if (baseController != null)
      {
        if (CurrentSession.Default.CurrentUser == null)
        {
          filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
              new { controller = "Account", action = "Register", area = "" }
            ));
        }
      }
    }
  }
}