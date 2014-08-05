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
using MLMExchange.Lib;
using MLMExchange.Models.Registration;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  [Auth(typeof(D_AdministratorRole))]
  public class UserRoleController : BaseController, IDataObjectCustomizableController<UserRoleModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult Edit(UserRoleModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      return View();
    }

    #region AllUserGrid__Paginate
    /// <summary>
    /// Пагинация для грида всех пользователей  
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public ActionResult AllUserGrid__Paginate(StoreRequestParameters parameters)
    {
      int start = parameters.Start;
      int limit = parameters.Limit;

      int totalUserCount;
      List<UserModel> userModelList = UserRoleList.GetUsers(start, limit, out totalUserCount).Select(x =>
        {
          UserModel model = new UserModel().Bind(x);
          model.UserRoles = new List<D_AbstractRole>();
          return model;
        }).ToList();

      Paging<UserModel> pageModelList = new Paging<UserModel>(userModelList, totalUserCount);

      return this.Store(pageModelList);
    }
    #endregion
  }
}