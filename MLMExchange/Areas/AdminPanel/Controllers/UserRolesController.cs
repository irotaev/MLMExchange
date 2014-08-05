using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MLMExchange.Controllers;
using MLMExchange.Areas.AdminPanel.Models;
using Logic;
using Ext.Net;
using Ext.Net.MVC;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  public class UserRolesController : BaseController, IDataObjectCustomizableController<UserRolesModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult Edit(UserRolesModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      return View();
    }

    public ActionResult Paginate(StoreRequestParameters parameters)
    {
      var output = this.Store(UserRolesModel.UserPaging(parameters));

      return output;
    }

    public ActionResult GetUsers(int start = 0, int limit = 15)
    {
      int counter = _NHibernateSession.QueryOver<D_User>().RowCount();

      return Json(new {
        totalUsers = counter,
        startUsers = start,
        limitUsers = limit
      }, JsonRequestBehavior.AllowGet);
    }
  }

}