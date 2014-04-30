using Logic;
using MLMExchange.Controllers;
using MLMExchange.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using NHibernate.Linq;
using MLMExchange.Lib.Exception;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  [Auth]
  public class BuyingMyCryptRequestController : BaseController
  {
    /// <summary>
    /// Проверочный платеж
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult CheckPayment(long buyingMyCryptRequestId)
    {
      BuyingMyCryptRequest buyingRequest = MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<BuyingMyCryptRequest>()
        .Where(x => x.State == BuyingMyCryptRequestState.Accepted && x.Id == buyingMyCryptRequestId && x.Buyer.Id == CurrentSession.Default.CurrentUser.Id).FirstOrDefault();

      if (buyingRequest == null)
        throw new UserVisible__WrongParametrException("buyingMyCryptRequestId");

      if (buyingRequest.CheckPayment == null)
        throw new MLMExchange.Lib.Exception.ApplicationException(String.Format("Check payment for request {0} is not set", buyingRequest));

      if (buyingRequest.CheckPayment.State == BuyingMyCryptCheckPaymentState.Approved)
        throw new UserVisible__CurrentActionAccessDenied();

      //TODO:Rtv Прикрепить платежную систему
      buyingRequest.CheckPayment.State = BuyingMyCryptCheckPaymentState.Approved;

      return Redirect(Request.UrlReferrer.ToString());
    }
  }
}
