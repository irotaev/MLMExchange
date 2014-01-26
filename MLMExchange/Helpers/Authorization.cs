using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MLMExchange.Helpers
{
  /// <summary>
  /// Аттрибут проверки авторизации
  /// </summary>
  public class AuthAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);

      if (HttpContext.Current.Request.Cookies["_AUTHORIZE"] == null)
        filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
            new {controller = "Account", action = "Login"}
          ));
    }
  }
}