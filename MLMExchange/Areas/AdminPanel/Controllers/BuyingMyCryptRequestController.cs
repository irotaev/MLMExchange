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
    public void CheckPayment(long buyingMyCryptRequestId)
    {
      BuyingMyCryptRequest buyingRequest = MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<BuyingMyCryptRequest>()
        .Where(x => x.State == BuyingMyCryptRequestState.Accepted && x.Id == buyingMyCryptRequestId && x.Buyer.Id == CurrentSession.Default.CurrentUser.Id).FirstOrDefault();

      if (buyingRequest == null)
        throw new UserVisible__WrongParametrException("buyingMyCryptRequestId");

      if (buyingRequest.CheckBill == null || buyingRequest.CheckBill.User.Id != CurrentSession.Default.CurrentUser.Id)
        throw new MLMExchange.Lib.Exception.ApplicationException(String.Format("Check bill for request {0} is not set or belong to anouther user", buyingRequest));

      if (buyingRequest.CheckBill.PaymentState == BillPaymentState.Paid)
        throw new UserVisible__CurrentActionAccessDenied();

      Payment checkPayment = new Payment
      {
        RealMoneyAmount = 120,
        User = CurrentSession.Default.CurrentUser
      };

      buyingRequest.CheckBill.Payments.Add(checkPayment);

      //TODO:Rtv Прикрепить платежную систему
      buyingRequest.CheckBill.PaymentState = BillPaymentState.Paid;

      MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(buyingRequest);
    }
  }
}
