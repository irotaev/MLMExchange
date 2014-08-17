using Logic;
using MLMExchange.Areas.AdminPanel.Models.User;
using MLMExchange.Controllers;
using MLMExchange.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using MLMExchange.Lib.Exception;
using NHibernate.Linq;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  /// <summary>
  /// Контроллер заявки на участие в торгах
  /// </summary>
  [Auth]
  public class BiddingParticipateApplicationController : BaseController
  {
    /// <summary>
    /// Подать заявку на участие в торгах
    /// </summary>
    /// <param name="model">Модель подачи заявки</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult BiddingParticipateApplicationApply(BiddingParticipateApplicationModel model)
    {
      if (!RoleTypeAccessLevel.IsUserHasAccessToTradeSystem(CurrentSession.Default.CurrentUser))
        throw new UserVisible__CurrentActionAccessDenied();

      if (model.MyCryptCount == null)
        throw new Logic.Lib.UserVisibleException(MLMExchange.Properties.ResourcesA.Exception_ModelInvalid);

      ModelState.Clear();

      UpdateModel<BiddingParticipateApplicationModel>(model);

      if (ModelState.IsValid)
      {
        SystemSettings systemSettings = SystemSettings.GetCurrentSestemSettings();

        if (((User)MLMExchange.Lib.CurrentSession.Default.CurrentUser).isUserBalanceLimited(model.MyCryptCount.Value))
          throw new Logic.Lib.UserVisibleException("Limit of the balance!");

        if (!(model.MyCryptCount <= MLMExchange.Lib.CurrentSession.Default.CurrentUser.Roles.Where(x => (x as D_UserRole) != null).Cast<D_UserRole>().FirstOrDefault().MyCryptCount))
          throw new UserVisible__WrongParametrException("MyCryptCount");

        if (!(model.MyCryptCount <= systemSettings.LogicObject.MaxMyCryptCount))
          throw new UserVisible__WrongParametrException("MyCryptCount");

        if (model.MyCryptCount <= 300)
          throw new UserVisible__WrongParametrException("MyCryptCount");

        D_BiddingParticipateApplication biddingApplication = model.UnBind((D_BiddingParticipateApplication)null);

        _NHibernateSession.SaveOrUpdate(biddingApplication);
      }
      else
      {
        throw new Logic.Lib.UserVisibleException(MLMExchange.Properties.ResourcesA.Exception_ModelInvalid);
      }

      return Redirect(Request.UrlReferrer.ToString());
    }

    /// <summary>
    /// Отклонить заявку
    /// </summary>
    /// <param name="buyingMyCryptRequestId">Id запроса на покупку my-crypt</param>
    /// <returns></returns>
    public ActionResult Denied(long buyingMyCryptRequestId)
    {
      BuyingMyCryptRequest buyingMyCryptRequest = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<BuyingMyCryptRequest>().Where(x => x.Id == buyingMyCryptRequestId).List().FirstOrDefault();

      if (buyingMyCryptRequest == null)
        throw new UserVisible__WrongParametrException("biddingParticipateApplicationId");

      if (buyingMyCryptRequest.SellerUser.Id != CurrentSession.Default.CurrentUser.Id)
        throw new UserVisible__CurrentActionAccessDenied();

      if (buyingMyCryptRequest.State != BuyingMyCryptRequestState.AwaitingConfirm)
        throw new UserVisible__CurrentActionAccessDenied();

      buyingMyCryptRequest.State = BuyingMyCryptRequestState.Denied;

      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(buyingMyCryptRequest);

      return Redirect(Request.UrlReferrer.ToString());
    }

    /// <summary>
    /// Подтвердить заявку
    /// </summary>
    /// <param name="buyingMyCryptRequestId">Id запроса на покупку my-crypt</param>
    /// <param name="buyerId">Id покупателя</param>
    /// <returns></returns>
    public ActionResult Accept(long buyingMyCryptRequestId)
    {
      BuyingMyCryptRequest buyingMyCryptRequest = _NHibernateSession.QueryOver<BuyingMyCryptRequest>().Where(x => x.Id == buyingMyCryptRequestId).List().FirstOrDefault();

      if (buyingMyCryptRequest == null)
        throw new UserVisible__WrongParametrException("biddingParticipateApplicationId");

      if (buyingMyCryptRequest.SellerUser.Id != CurrentSession.Default.CurrentUser.Id)
        throw new UserVisible__CurrentActionAccessDenied();

      if (buyingMyCryptRequest.State != BuyingMyCryptRequestState.AwaitingConfirm)
        throw new UserVisible__CurrentActionAccessDenied();

      TradingSession.OpenTradingSession(buyingMyCryptRequest);

      return Redirect(Request.UrlReferrer.ToString());
    }

    /// <summary>
    /// Принять комиссионный платеж продавцу
    /// </summary>
    /// <param name="tradeSessionId">Id торговой сессии</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult SellerInterestRatePayment_Accepted(long tradeSessionId)
    {
      D_TradingSession tradingSession = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<D_TradingSession>().Where(x => x.Id == tradeSessionId).List().FirstOrDefault();

      if (tradingSession == null)
        throw new UserVisible__WrongParametrException("tradeSessionId");

      if (tradingSession.BiddingParticipateApplication.Seller.Id != CurrentSession.Default.CurrentUser.Id)
        throw new UserVisible__CurrentActionAccessDenied();

      tradingSession.SallerInterestRateBill.PaymentState = BillPaymentState.Paid;
      tradingSession.BiddingParticipateApplication.State = BiddingParticipateApplicationState.Closed;
      ((BiddingParticipateApplication)tradingSession.BiddingParticipateApplication).WriteOfBuyedMyCrypt();

      tradingSession.State = TradingSessionStatus.NeedEnsureProfibility;

      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(tradingSession);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }

    /// <summary>
    /// Отклонить комиссионный платеж продавцу
    /// </summary>
    /// <param name="tradeSessionId">Id торговой сессии</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult SellerInterestRatePayment_Denied(long tradeSessionId)
    {
      D_TradingSession tradingSession = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<D_TradingSession>().Where(x => x.Id == tradeSessionId).List().FirstOrDefault();

      if (tradingSession == null)
        throw new UserVisible__WrongParametrException("tradeSessionId");

      if (tradingSession.BiddingParticipateApplication.Seller.Id != CurrentSession.Default.CurrentUser.Id)
        throw new UserVisible__CurrentActionAccessDenied();

      tradingSession.SallerInterestRateBill.PaymentState = BillPaymentState.NotPaid;
      tradingSession.State = TradingSessionStatus.Baned;

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }
  }
}
