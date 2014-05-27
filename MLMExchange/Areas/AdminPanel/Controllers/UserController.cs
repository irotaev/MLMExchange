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
using NHibernate.Linq;
using MLMExchange.Areas.AdminPanel.Models;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  [Auth(typeof(D_AdministratorRole), typeof(D_UserRole))]
  public class UserController : BaseController, IDataObjectCustomizableController<UserModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      if (actionSettings.objectId == null)
        throw new UserVisible__ArgumentNullException("objerctId");

      D_User user = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_User>().Where(x => x.Id == actionSettings.objectId).FirstOrDefault();

      if (user == null)
        throw new UserVisible__WrongParametrException("objectId");

      return View(new UserModel().Bind(user));
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      List<UserModel> model = new List<UserModel>();

      model.AddRange(Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_User>().Select(u => new UserModel().Bind(u)).Cast<UserModel>());

      return View(model);
    }

    /// <summary>
    /// Редактирование информации о пользователе
    /// </summary>
    /// <param name="model">Модель пользователя</param>
    /// <returns></returns>
    public ActionResult Edit(UserModel model, BaseEditActionSettings actionSettings)
    {
      Logic.D_User user = CurrentSession.Default.CurrentUser;

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

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(user);
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

          D_AddMyCryptTransaction addMyCryptTransaction = model.UnBind();
          addMyCryptTransaction.User = CurrentSession.Default.CurrentUser;

          Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(addMyCryptTransaction);
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
      NHibernate.ISession session = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;

      ControlPanelModel model = new ControlPanelModel();

      #region Заявка на продажу my-crypt
      model.BiddingParticipateApplicationStateModel = new BiddingParticipateApplicationStateModel();

      BiddingParticipateApplication biddingParticipateApplication = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<BiddingParticipateApplication>().Where(x => x.Seller.Id == CurrentSession.Default.CurrentUser.Id 
                                                            && x.State != BiddingParticipateApplicationState.NA
                                                            && x.State != BiddingParticipateApplicationState.Closed).List().FirstOrDefault();

      if (biddingParticipateApplication == null)
      {
        model.BiddingParticipateApplicationStateModel.State = ApplicationState.NotFiled;
        model.BiddingParticipateApplicationStateModel.BiddingParticipateApplicationModel = new BiddingParticipateApplicationModel();
      }
      else
      {
        BuyingMyCryptRequest acceptedBuyinMyCryptRequest = biddingParticipateApplication.BuyingMyCryptRequests.Where(x => x.State == BuyingMyCryptRequestState.Accepted).FirstOrDefault();

        if (acceptedBuyinMyCryptRequest != null)
        {
          model.BiddingParticipateApplicationStateModel.State = ApplicationState.Accepted;
          model.BiddingParticipateApplicationStateModel.BiddingParticipateApplicationAcceptedModel = new BiddingParticipateApplicationAcceptedModel
          {
            Buyer = new UserModel().Bind(acceptedBuyinMyCryptRequest.Buyer),
            IsSellerInterestRate_NeedSubstantialMoney = acceptedBuyinMyCryptRequest.TradingSession.SallerInterestRateBill.IsNeedSubstantialMoney,
            TradingSessionId = acceptedBuyinMyCryptRequest.TradingSession.Id
          };
        }
        else
        {
          IList<BuyingMyCryptRequest> buyingMyCryptRequests = biddingParticipateApplication.BuyingMyCryptRequests;

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
      }
      #endregion

      #region TradingSession
      {
        model.TradingSessionModel = new Models.TradingSessionModel();

        D_TradingSession tradingSession = session.Query<D_TradingSession>()
          .Where(x => x.BuyingMyCryptRequest.Buyer.Id == CurrentSession.Default.CurrentUser.Id 
                      && x.State != TradingSessionStatus.NA
                      && x.State != TradingSessionStatus.Closed)
          .FirstOrDefault();

        if (tradingSession != null)
          model.TradingSessionModel.Bind((TradingSession)tradingSession);
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

      IList<BiddingParticipateApplication> biddingApplications = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<BiddingParticipateApplication>()
        .Where(x => x.State == BiddingParticipateApplicationState.Filed && x.BuyingMyCryptRequests.All(r => r.Buyer.Id != CurrentSession.Default.CurrentUser.Id)).ToList();        
      
      foreach (var biddingApplication in biddingApplications)
      {
        BiddingParticipateApplicationModel biddingApplicationModel = new BiddingParticipateApplicationModel();

        biddingApplicationModel.Bind(biddingApplication);

        model.ActiveSales.Add(biddingApplicationModel);
      }
      #endregion

      #region Заполняю историю заявок, на которые откликнулс данный пользователь
      model.HistoryApplication = new List<BuyingMyCryptRequestModel>();

      IList<BuyingMyCryptRequest> buyingMyCryptRequestsHistory = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
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

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(buyingMyCryptRequest);
      }
      else
      {
        throw new UserVisibleException(MLMExchange.Properties.ResourcesA.Exception_ModelInvalid);
      }

      return Redirect(Request.UrlReferrer.ToString());
    }    
  }
}