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
  public class RandomWordsController : BaseController, IDataObjectCustomizableController<RandomWordsModel, BaseBrowseActionSettings, MLMExchange.Areas.AdminPanel.Controllers.RandomWordsController.CustomEditActionSettings, BaseListActionSetings>
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

    public ActionResult Add(RandomWordsModel model, CustomEditActionSettings actionSettings)
    {
      ModelState.Clear();

      if (actionSettings.Method != "GET")
      {
        TryUpdateModel<RandomWordsModel>(model);

        if (ModelState.IsValid)
        {
          D_RandomWord d_randomwords = model.UnBind();

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(d_randomwords);

        }
      }

      if (actionSettings.AsPartial != null)
      {
        if (actionSettings.AsPartial.Value)
          ViewData["AsPartial"] = "True";
      }

      return View(model);
    }

    public ActionResult Edit(RandomWordsModel model, CustomEditActionSettings actionSettings)
    {
      ModelState.Clear();

      D_RandomWord d_randomwords = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_RandomWord>().Where(x => x.Id == actionSettings.objectId).FirstOrDefault();

      if (model.Id == null)
      {
        model.Bind(d_randomwords);
      }
      else if (actionSettings.Method != "GET")
      {
        TryUpdateModel<RandomWordsModel>(model);

        if (ModelState.IsValid)
        {
          d_randomwords = model.UnBind(d_randomwords);

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(d_randomwords);

          model.Bind(d_randomwords);
        }
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
      IList<D_RandomWord> wordsModel = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<D_RandomWord>().Where(x => x.Id != null).List();

      List<RandomWordsModel> randomWordsList = new List<RandomWordsModel>();

      foreach (var words in wordsModel)
      {
        RandomWordsModel randomWordsModel = new RandomWordsModel();
        
        randomWordsModel.Bind(words);

        randomWordsList.Add(randomWordsModel);
      }

      return View(randomWordsList);
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

    public class CustomEditActionSettings : BaseEditActionSettings
    {
      public string Method { get; set; }
    }
  }
}
