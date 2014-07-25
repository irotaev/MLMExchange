using MLMExchange.Areas.AdminPanel.Models;
using MLMExchange.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using NHibernate.Linq;
using Logic;
using MLMExchange.Lib.Exception;
using MLMExchange.Lib;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  [Auth(typeof(D_AdministratorRole))]
  public class RandomWordsController : BaseController, IDataObjectCustomizableController<RandomWordsModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      if (actionSettings.objectId == null)
        throw new Logic.Lib.UserVisible__ArgumentNullException("objectId");

      D_RandomWord randomWords = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_RandomWord>().Where(x => x.Id == actionSettings.objectId).FirstOrDefault();

      if (randomWords == null)
        throw new Logic.Lib.UserVisible__ArgumentNullException("randomWords");

      if (actionSettings.AsPartial != null)
      {
        if (actionSettings.AsPartial.Value)
          ViewData["AsPartial"] = "True";
      }

      return View(new RandomWordsModel().Bind(randomWords));
    }

    public ActionResult Edit(RandomWordsModel model, BaseEditActionSettings actionSettings)
    {
      ModelState.Clear();

      if (ControllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        TryUpdateModel<RandomWordsModel>(model);

        if (ModelState.IsValid)
        {
          D_RandomWord d_randomwords = model.UnBind();

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(d_randomwords);
        }
      }
      else
      {
        D_RandomWord modelLogic = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_RandomWord>().Where(x => x.Id == actionSettings.objectId).FirstOrDefault();

        if (modelLogic != null)
          model.Bind(modelLogic);
      }

      if (actionSettings.AsPartial != null)
      {
        if (actionSettings.AsPartial.Value)
          ViewData["AsPartial"] = "True";
      }

      return View(model);
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      IEnumerable<RandomWordsModel> wordsModel = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_RandomWord>().Where(x => x.Id != null).Select(x => new RandomWordsModel().Bind(x));

      return View(wordsModel);
    }

    [HttpPost]
    public ActionResult Delete(long wordsId)
    {
      var session = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;

      D_RandomWord d_randomWords = session.Query<D_RandomWord>().Where(x => x.Id == wordsId).FirstOrDefault();

      if (d_randomWords == null)
        throw new UserVisible__WrongParametrException("wordsId");

      session.Delete(d_randomWords);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }
  }

}
