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

        MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(biddingApplication);
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
      BuyingMyCryptRequest buyingMyCryptRequest = MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<BuyingMyCryptRequest>().Where(x => x.Id == buyingMyCryptRequestId).List().FirstOrDefault();

      if (buyingMyCryptRequest == null)
        throw new UserVisible__WrongParametrException("biddingParticipateApplicationId");

      if (buyingMyCryptRequest.SellerUser.Id != CurrentSession.Default.CurrentUser.Id)
        throw new UserVisible__CurrentActionAccessDenied();

      if (buyingMyCryptRequest.State != BuyingMyCryptRequestState.AwaitingConfirm)
        throw new UserVisible__CurrentActionAccessDenied();

      buyingMyCryptRequest.State = BuyingMyCryptRequestState.Denied;
      buyingMyCryptRequest.BiddingParticipateApplication.State = BiddingParticipateApplicationState.Denied;

      MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(buyingMyCryptRequest);

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
      BuyingMyCryptRequest buyingMyCryptRequest = MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .QueryOver<BuyingMyCryptRequest>().Where(x => x.Id == buyingMyCryptRequestId).List().FirstOrDefault();

      if (buyingMyCryptRequest == null)
        throw new UserVisible__WrongParametrException("biddingParticipateApplicationId");

      if (buyingMyCryptRequest.SellerUser.Id != CurrentSession.Default.CurrentUser.Id)
        throw new UserVisible__CurrentActionAccessDenied();

      if (buyingMyCryptRequest.State != BuyingMyCryptRequestState.AwaitingConfirm)
        throw new UserVisible__CurrentActionAccessDenied();

      buyingMyCryptRequest.State = BuyingMyCryptRequestState.Accepted;
      buyingMyCryptRequest.BiddingParticipateApplication.State = BiddingParticipateApplicationState.Accepted;

      MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(buyingMyCryptRequest);

      return Redirect(Request.UrlReferrer.ToString());
    }
  }
}
