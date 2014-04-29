using Logic;
using MLMExchange.Areas.AdminPanel.Models.User;
using MLMExchange.Areas.AdminPanel.Models.User.SalesPeople;
using MLMExchange.Controllers;
using MLMExchange.Lib;
using MLMExchange.Lib.Exception;
using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  [Auth]
  public class UserController : BaseController
  {
    /// <summary>
    /// Редактирование информации о пользователе
    /// </summary>
    /// <param name="model">Модель пользователя</param>
    /// <returns></returns>
    public ActionResult Edit(UserModel model)
    {
      Logic.User user = CurrentSession.Default.CurrentUser;

      if (model.Id == null)
      {
        model.Bind(user);
      }
      else if (ControllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        if (ModelState.IsValid)
        {
          #region Сохраняю фото
          if (model.Photo != null)
          {
            string filePath = MLMExchange.Lib.Image.Image.SaveImage(model.Photo, Server);

            model.PhotoRelativePath = filePath;
          }
          #endregion

          user = model.UnBind(user);

          MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(user);
          //transaction.Commit();
        }
      }

      return View(model);
    }

    /// <summary>
    /// Добавить/пополнить my-crypt
    /// </summary>
    /// <param name="model">Модель добавления myc-rypt</param>
    /// <returns></returns>
    public ActionResult AddMyCrypt(AddMyCryptModel model)
    {
      ModelState.Clear();

      if (ControllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        if (ModelState.IsValid)
        {
          #region Сохраняю фото
          if (model.Image != null)
          {
            string filePath = MLMExchange.Lib.Image.Image.SaveImage(model.Image, Server);

            model.ImageRelativePath = filePath;
          }
          #endregion

          AddMyCryptTransaction addMyCryptTransaction = model.UnBind();

          MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(addMyCryptTransaction);
          //transaction.Commit();

          return View("_AddMyCrypt_Success", model);
        }
      }

      return View(model);
    }

    /// <summary>
    /// Панель управления пользователя
    /// </summary>
    /// <returns></returns>
    public ActionResult ControlPanel()
    {
      ControlPanelModel model = new ControlPanelModel();

      #region Заявка на продажу my-crypt
      model.BiddingParticipateApplicationStateModel = new BiddingParticipateApplicationStateModel();

      BiddingParticipateApplication biddingParticipateApplication = MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<BiddingParticipateApplication>().Where(x => x.Seller.Id == CurrentSession.Default.CurrentUser.Id && x.State == BiddingParticipateApplicationState.Filed).List().FirstOrDefault();

      if (biddingParticipateApplication == null || biddingParticipateApplication.State != BiddingParticipateApplicationState.Filed)
      {
        model.BiddingParticipateApplicationStateModel.State = ApplicationState.NotFiled;
        model.BiddingParticipateApplicationStateModel.BiddingParticipateApplicationModel = new BiddingParticipateApplicationModel();
      }
      else
      {
        IList<BuyingMyCryptRequest> buyingMyCryptRequests = MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
          .QueryOver<BuyingMyCryptRequest>().Where(x => x.SellerUser.Id == CurrentSession.Default.CurrentUser.Id && x.State == BuyingMyCryptRequestState.AwaitingConfirm).List();

        if (buyingMyCryptRequests == null || buyingMyCryptRequests.Count == 0)
        {
          model.BiddingParticipateApplicationStateModel.State = ApplicationState.ExpectsBuyers;
        }
        else
        {
          model.BiddingParticipateApplicationStateModel.State = ApplicationState.BuyerFound;
          model.BiddingParticipateApplicationStateModel.BiddingParticipateApplicationBuyerFoundModel = new BiddingParticipateApplicationBuyerFoundModel
          {
            BuyRequests = new List<BuyingMyCryptRequestModel>()
          };

          foreach (var request in buyingMyCryptRequests)
          {
            BuyingMyCryptRequestModel buyRequest = new BuyingMyCryptRequestModel().Bind(request);

            model.BiddingParticipateApplicationStateModel.BiddingParticipateApplicationBuyerFoundModel.BuyRequests.Add(buyRequest);
          }
        }
      }
      #endregion

      return View(model);
    }

    /// <summary>
    /// Продавцы
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ActionResult SalesPeople()
    {
      SalesPeopleModel model = new SalesPeopleModel();

      #region Заполняю активных продавцов
      model.ActiveSales = new List<BiddingParticipateApplicationModel>();

      IList<BiddingParticipateApplication> biddingApplications = MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<BiddingParticipateApplication>()
        .Where(x1 => x1.State == BiddingParticipateApplicationState.Filed)
        .WhereRestrictionOn(x2 => x2.BuyingMyCryptRequests.Select(r => r.Buyer.Id)).Not.IsIn(new long[] { CurrentSession.Default.CurrentUser.Id }).List();
      AAAAAAAAAAAAAAAAAAAAAA
      foreach (var biddingApplication in biddingApplications)
      {
        BiddingParticipateApplicationModel biddingApplicationModel = new BiddingParticipateApplicationModel();

        biddingApplicationModel.Bind(biddingApplication);

        model.ActiveSales.Add(biddingApplicationModel);
      }
      #endregion

      #region Заполняю историю заявок, на которые откликнулс данный пользователь
      model.HistoryApplication = new List<BuyingMyCryptRequestModel>();

      IList<BuyingMyCryptRequest> buyingMyCryptRequestsHistory = MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<BuyingMyCryptRequest>().Where(x => x.Buyer.Id == CurrentSession.Default.CurrentUser.Id).List();

      foreach (var request in buyingMyCryptRequestsHistory)
      {
        BuyingMyCryptRequestModel buyingRequestModel = new BuyingMyCryptRequestModel();

        buyingRequestModel.Bind(request);

        model.HistoryApplication.Add(buyingRequestModel);
      }
      #endregion

      return View(model);
    }

    /// <summary>
    /// Запрос на покупку my-crypt у продовца
    /// </summary>
    /// <param name="model">Модель</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult BuyMyCrypt(BuyingMyCryptRequestModel model)
    {
      ModelState.Clear();

      TryUpdateModel<BuyingMyCryptRequestModel>(model);

      if (ModelState.IsValid)
      {
        BuyingMyCryptRequest buyingMyCryptRequest = model.UnBind(((BuyingMyCryptRequest)null));

        MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(buyingMyCryptRequest);
      }
      else
      {
        throw new UserVisibleException(MLMExchange.Properties.ResourcesA.Exception_ModelInvalid);
      }

      return Redirect(Request.UrlReferrer.ToString());
    }
  }
}