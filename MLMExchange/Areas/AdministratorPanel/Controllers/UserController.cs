using Ext.Net;
using Logic;
using MLMExchange.Lib;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using Ext.Net.MVC;

namespace MLMExchange.Areas.AdministratorPanel.Controllers
{
  [Auth(typeof(D_AdministratorRole))]
  public class UserController : MLMExchange.Areas.AdminPanel.Controllers.UserController
  {
    public override ActionResult List(MLMExchange.Controllers.BaseListActionSetings actionSettings)
    {
      List<UserModel> model = new List<UserModel>();

      return View(model);
    }

    public ActionResult List__Paginator(StoreRequestParameters parameters)
    {
      List<D_User> users = _NHibernateSession.Query<D_User>().Skip(parameters.Start).Take(parameters.Limit).ToList();

      Paging<UserModel> pagingUsers = new Paging<UserModel>(users.Select(x => new UserModel().Bind(x)), _NHibernateSession.Query<D_User>().Count());

      return this.Store(pagingUsers);
    }
  }
}