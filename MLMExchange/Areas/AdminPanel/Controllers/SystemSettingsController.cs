using Logic;
using MLMExchange.Areas.AdminPanel.Models;
using MLMExchange.Controllers;
using MLMExchange.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using NHibernate.Linq;
using MLMExchange.Models.Registration;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  [Auth(typeof(D_AdministratorRole))]
  public class SystemSettingsController : BaseController, IDataObjectCustomizableController<SystemSettingsModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult Edit(SystemSettingsModel model, BaseEditActionSettings actionSettings)
    {
      ModelState.Clear();

      if (Request.RequestType == "POST")
      {
        TryUpdateModel<SystemSettingsModel>(model);

        if (ModelState.IsValid)
        {
          D_SystemSettings d_systemSettings = model.UnBind();

          D_User defaultSystemUserModel = _NHibernateSession.Query<D_User>().Where(x => x.Login == model.DefaultSystemUserName).FirstOrDefault();

          if (defaultSystemUserModel == null)
            throw new Logic.Lib.UserVisible__ArgumentNullException("DefaultSystemUserName");

          d_systemSettings.DefaultSystemUser = defaultSystemUserModel;
          
          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(d_systemSettings);
        }
      }
      else
      {
        SystemSettings systemSettings = SystemSettings.GetCurrentSestemSettings();
        
        if (systemSettings != null)
          model.Bind(systemSettings.LogicObject);
      }

      return View(model);
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      throw new NotImplementedException();
    }
  }
}
