using Logic;
using MLMExchange.Controllers;
using MLMExchange.Lib.Exception;
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
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
  public class AuthAttribute : ActionFilterAttribute
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="allowedRoles">Роли, которым открыт доступ</param>
    public AuthAttribute(params Type[] allowedRoles)
    {
      if (allowedRoles.Length > 0)
        _AllowedRoleTypes.AddRange(allowedRoles);
    }

    private readonly List<Type> _AllowedRoleTypes = new List<Type>();

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);

      BaseController baseController = filterContext.Controller as BaseController;

      if (baseController != null)
      {
        D_User currentUser = CurrentSession.Default.CurrentUser;

        if (currentUser == null)
        {
          filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
              new { controller = "Account", action = "Register", area = "" }
            ));
        }
        else if (!currentUser.IsUserRegistrationConfirm 
          && (string)filterContext.Controller.ControllerContext.RouteData.Values["action"] != "Confirm"
          && (string)filterContext.Controller.ControllerContext.RouteData.Values["controller"] != "Account")
        {
          filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
              new { controller = "Account", action = "Confirm", area = "" }
            ));
        }
        else if (_AllowedRoleTypes.Count > 0)
        {
          bool isAccessDenied = true;

          foreach (var role in currentUser.Roles)
          {
            if (_AllowedRoleTypes.Contains(((BaseObject)role).GetRealType()))
              isAccessDenied = false;
          }

          if (isAccessDenied)
            throw new UserVisible__CurrentActionAccessDenied(MLMExchange.Properties.ResourcesA.Action_AccessToController);
        }
      }
    }
  }
}