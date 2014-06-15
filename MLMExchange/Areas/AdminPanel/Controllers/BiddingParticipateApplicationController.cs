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
      ModelState.Clear();

      UpdateModel<BiddingParticipateApplicationModel>(model);

      if (ModelState.IsValid)
      {
        BiddingParticipateApplication biddingApplication = model.UnBind((BiddingParticipateApplication)null);

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(biddingApplication);
      }
      else
      {
        throw new UserVisibleException(MLMExchange.Properties.ResourcesA.Exception_ModelInvalid);
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
      BuyingMyCryptRequest buyingMyCryptRequest = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<BuyingMyCryptRequest>().Where(x => x.Id == buyingMyCryptRequestId).List().FirstOrDefault();

      if (buyingMyCryptRequest == null)
        throw new UserVisible__WrongParametrException("biddingParticipateApplicationId");

      if (buyingMyCryptRequest.SellerUser.Id != CurrentSession.Default.CurrentUser.Id)
        throw new UserVisible__CurrentActionAccessDenied();

      if (buyingMyCryptRequest.State != BuyingMyCryptRequestState.AwaitingConfirm)
        throw new UserVisible__CurrentActionAccessDenied();

      #region Создание торговой сессии
      D_TradingSession tradingSession = new D_TradingSession
      {
        BuyingMyCryptRequest = buyingMyCryptRequest,
        BiddingParticipateApplication = buyingMyCryptRequest.BiddingParticipateApplication,
        State = TradingSessionStatus.Open,
        SystemSettings = SystemSettings.GetCurrentSestemSettings().LogicObject
      };
      #endregion

      #region Счет проверочного платежа
      D_Bill checkBill = new D_Bill
      {
        MoneyAmount = ((TradingSession)tradingSession).CalculateCheckPaymentMoneyAmount(),
        PaymentState = BillPaymentState.WaitingPayment,
        Payer = tradingSession.BuyingMyCryptRequest.Buyer
      };

      tradingSession.CheckBill = checkBill;
      #endregion

      #region Счет сбора продавцу
      D_Bill sallerInterestRateBill = new D_Bill
      {
        MoneyAmount = ((TradingSession)tradingSession).CalculateSallerInterestRateMoneyAmount(),
        Payer = tradingSession.BuyingMyCryptRequest.Buyer,
        PaymentState = BillPaymentState.WaitingPayment
      };

      tradingSession.SallerInterestRateBill = sallerInterestRateBill;
      #endregion

      buyingMyCryptRequest.State = BuyingMyCryptRequestState.Accepted;
      buyingMyCryptRequest.BiddingParticipateApplication.State = BiddingParticipateApplicationState.Accepted;

      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(tradingSession);

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
