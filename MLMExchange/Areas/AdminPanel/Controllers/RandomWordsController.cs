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
  public class RandomWordsController : BaseController, IDataObjectCustomizableController<RandomWordsModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult Edit(RandomWordsModel model, BaseEditActionSettings actionSettings)
    {
      ModelState.Clear();

      if (Request.RequestType == "POST")
      {
        TryUpdateModel<RandomWordsModel>(model);

        if (ModelState.IsValid)
        {
          D_RandomWord d_randomwords = model.UnBind();

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(d_randomwords);
        }
      }

      return View(model);
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      throw new NotImplementedException();
    }
  }

}
