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

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(d_systemSettings);
        }
      }
      else
      {
        DateTime maxCreatedDate = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
          .Query<D_SystemSettings>().Max(x => x.CreationDateTime);

        D_SystemSettings d_systemSettings = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
          .Query<D_SystemSettings>().Where(x => x.CreationDateTime == maxCreatedDate).FirstOrDefault();

        if (d_systemSettings != null)
          model.Bind(d_systemSettings);
      }

      return View(model);
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      throw new NotImplementedException();
    }
  }
}
