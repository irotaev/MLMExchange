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
using System.Collections;
using Ext.Net.MVC;
using MLMExchange.Lib.Image;
using Ext.Net;
using MLMExchange.Areas.AdminPanel.Models.PaymentSystem;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  [Auth(typeof(D_AdministratorRole), typeof(D_UserRole))]
  public class UserController : BaseController, IDataObjectCustomizableController<UserModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      if (actionSettings.objectId == null)
        throw new Logic.Lib.UserVisible__ArgumentNullException("objerctId");

      D_User user = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_User>().Where(x => x.Id == actionSettings.objectId).FirstOrDefault();

      if (user == null)
        throw new UserVisible__WrongParametrException("objectId");

      if (Request.IsAjaxRequest())
        ViewData["AsPartial"] = "True";

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
      ModelState.Clear();

      Logic.D_User user = CurrentSession.Default.CurrentUser;

      if (model.Id == null)
      {
        model.Bind(user);
      }
      else if (ControllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        TryUpdateModel<UserModel>(model);

        if (ModelState.IsValid)
        {
          #region Сохраняю фото
          if (model.Photo != null)
          {
            string filePath = MLMExchange.Lib.Image.Image.SaveImage(model.Photo, Server);

            model.PhotoRelativePath = filePath;
          }
          #endregion

          model.EditValidate(ModelState, user);

          if (ModelState.IsValid)
          {
            user = model.UnBind(user);

            Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(user);
          }

          model.Bind(user);
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

      D_AddMyCryptTransaction sendedTransaction = _NHibernateSession.Query<D_AddMyCryptTransaction>().Where(x => x.User.Id == CurrentSession.Default.CurrentUser.Id 
        && x.State == AddMyCryptTransactionState.NA).FirstOrDefault();

      if (sendedTransaction != null)
        return View("AddMyCrypt_AlreadySended", new AddMyCryptModel().Bind(sendedTransaction));

      if (ControllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        if (ModelState.IsValid)
        {
          // TODO: Пересмотреть валидацию фото
          #region Валидация фото

          MLMExchange.Lib.Image.Image.Validation imageValidation = new MLMExchange.Lib.Image.Image.Validation(model.Image);
          imageValidation.ValidateImage();

          #endregion

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

      D_BiddingParticipateApplication biddingParticipateApplication = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<D_BiddingParticipateApplication>().Where(x => x.Seller.Id == CurrentSession.Default.CurrentUser.Id
                                                            && x.State != BiddingParticipateApplicationState.NA
                                                            && x.State != BiddingParticipateApplicationState.Closed).List().FirstOrDefault();
      if (biddingParticipateApplication == null)
      {
        model.BiddingParticipateApplicationStateModel.State = ApplicationState.NotFiled;
        model.BiddingParticipateApplicationStateModel.BiddingParticipateApplicationModel = new BiddingParticipateApplicationModel();
      }
      else if (biddingParticipateApplication.TradingSession != null && biddingParticipateApplication.TradingSession.State == TradingSessionStatus.Baned)
      {
        model.BiddingParticipateApplicationStateModel.BiddingParticipateApplicationModel = new BiddingParticipateApplicationModel().Bind(biddingParticipateApplication);
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
            TradingSessionId = acceptedBuyinMyCryptRequest.TradingSession.Id,
            PaymentIds = acceptedBuyinMyCryptRequest.TradingSession.SallerInterestRateBill.Payments.Select(x => x.Id)
          };
        }
        else
        {
          IList<BuyingMyCryptRequest> buyingMyCryptRequests = biddingParticipateApplication.BuyingMyCryptRequests.Where(x => x.State == BuyingMyCryptRequestState.AwaitingConfirm).ToList();

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

      #region UserControlBlockModel
      model.UserControlBlockModel = new UserControlBlockModel().Bind((User)CurrentSession.Default.CurrentUser);
      #endregion

      return View(model);
    }

    #region Пагинация Controlpanel
    public ActionResult GridTradingSession__Paginate(StoreRequestParameters parameters)
    {
      int start = parameters.Start;
      int limit = parameters.Limit;
      int totalCount;

      TradingSessionModel model = new Models.TradingSessionModel();

      D_TradingSession tradingSession = _NHibernateSession.Query<D_TradingSession>()
          .Where(x => x.BuyingMyCryptRequest.Buyer.Id == CurrentSession.Default.CurrentUser.Id
                      && x.State != TradingSessionStatus.NA
                      && x.State != TradingSessionStatus.Closed)
          .FirstOrDefault();

      List<YieldSessionPaymentAcceptor> yeildAccepterModels = new List<YieldSessionPaymentAcceptor>();

      if (tradingSession != null)
        yeildAccepterModels.AddRange(TradingSessionModel.GetYieldSessionPaymentAcceptors((TradingSession)tradingSession, (uint?)start, (uint?)limit));

      totalCount = model.YieldSessionPaymentAcceptors.Count();

      Paging<YieldSessionPaymentAcceptor> pageModelList = new Paging<YieldSessionPaymentAcceptor>(yeildAccepterModels, totalCount);

      return this.Store(pageModelList);
    }

    public ActionResult GridTradingSessionNeedProfit__Paginate(StoreRequestParameters parameters)
    {
      int start = parameters.Start;
      int limit = parameters.Limit;
      int totalCount;

      TradingSessionModel model = new TradingSessionModel();

      D_TradingSession tradingSession = _NHibernateSession.Query<D_TradingSession>()
          .Where(x => x.BuyingMyCryptRequest.Buyer.Id == CurrentSession.Default.CurrentUser.Id
                      && x.State != TradingSessionStatus.NA
                      && x.State != TradingSessionStatus.Closed)
          .FirstOrDefault();

      List<BillModel> needProfitBillsList = new List<BillModel>();

      if (tradingSession != null)
        needProfitBillsList.AddRange(TradingSessionModel.GetNeedProfitBills((TradingSession)tradingSession, (uint?)start, (uint?)limit));

      totalCount = needProfitBillsList.Count();

      Paging<BillModel> pageNeedProfitBillsList = new Paging<BillModel>(needProfitBillsList, totalCount);

      return this.Store(pageNeedProfitBillsList);
    }
    #endregion

    #region SalesPeople
    /// <summary>
    /// Продавцы
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ActionResult SalesPeople()
    {
      SalesPeopleModel model = new SalesPeopleModel();

      model.ActiveSales = new List<BiddingParticipateApplicationModel>();
      model.HistoryApplication = new List<BuyingMyCryptRequestModel>();

      return View(model);
    }

    #region Пагинация SalesPeople
    /// <summary>
    /// Получить историю заявок, на которые откликнулся пользователь.
    /// Используется Ext.NET
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ActionResult ActiveSalesGrid__Paginate(StoreRequestParameters parameters)
    {
      int start = parameters.Start;
      int limit = parameters.Limit;

      int totalCount;

      IList<D_BiddingParticipateApplication> biddingApplicationList = _NHibernateSession.Query<D_BiddingParticipateApplication>()
        .Where(x => x.State == BiddingParticipateApplicationState.Filed && 
               x.BuyingMyCryptRequests.All(r => r.Buyer.Id != CurrentSession.Default.CurrentUser.Id))
        .Skip(start).Take(limit).ToList();

      totalCount = _NHibernateSession.Query<D_BiddingParticipateApplication>().Count();

      List<BiddingParticipateApplicationModel> activeSalesList = biddingApplicationList.Select(x =>
      {
        BiddingParticipateApplicationModel model = new BiddingParticipateApplicationModel().Bind(x);
        return model;
      }).ToList();

      Paging<BiddingParticipateApplicationModel> activeSalesPaging = new Paging<BiddingParticipateApplicationModel>(activeSalesList, totalCount);

      return this.Store(activeSalesPaging);
    }

    /// <summary>
    /// Получить историю покупок
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult HistoryApplicationGrid__Paginate(StoreRequestParameters parameters)
    {
      int start = parameters.Start;
      int limit = parameters.Limit;

      int totalCount;

      IOrderedEnumerable<BuyingMyCryptRequest> byuingRequstsList = _NHibernateSession.QueryOver<BuyingMyCryptRequest>()
        .Where(x => x.Buyer.Id == (CurrentSession.Default.CurrentUser.Id)).Skip(start).Take(limit).List().OrderBy(x => x.CreationDateTime);

      totalCount = _NHibernateSession.Query<BuyingMyCryptRequest>().Count();

      List<BuyingMyCryptRequestModel> requestsList = byuingRequstsList.Select(x =>
        {
          BuyingMyCryptRequestModel model = new BuyingMyCryptRequestModel().Bind(x);
          return model;
        }).ToList();

      Paging<BuyingMyCryptRequestModel> requestsPaging = new Paging<BuyingMyCryptRequestModel>(requestsList, totalCount);

      return this.Store(requestsPaging);
    }
    #endregion

    #endregion

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

        if (model.MyCryptCount > buyingMyCryptRequest.BiddingParticipateApplication.MyCryptCount)
          throw new UserVisible__WrongParametrException("model");

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(buyingMyCryptRequest);
      }
      else
      {
        throw new Logic.Lib.UserVisibleException(MLMExchange.Properties.ResourcesA.Exception_ModelInvalid);
      }

      return Redirect(Request.UrlReferrer.ToString());
    }
  }
}