using Logic;
using MLMExchange.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Providers.Entities;

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
        //if (HttpContext.Current.Request.Cookies["_AUTHORIZE"] == null || Boolean.Parse(HttpContext.Current.Request.Cookies["_AUTHORIZE"].Value) != true
        //  || filterContext.RequestContext.HttpContext.Session["Authorized"] == null || (bool)filterContext.RequestContext.HttpContext.Session["Authorized"] != true)
        if (baseController.CurrentSession.CurrentUser == null)
        {
          filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
              new { controller = "Account", action = "Register" }
            ));
        }
      }
    }
  }
}