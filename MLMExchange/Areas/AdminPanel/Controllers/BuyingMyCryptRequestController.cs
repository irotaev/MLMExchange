﻿using Logic;
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
using MLMExchange.Lib.EntityLogic;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  [Auth]
  public class BuyingMyCryptRequestController : BaseController
  {
    /// <summary>
    /// Проверочный платеж
    /// </summary>
    /// <param name="buyingMyCryptRequestId">Id запроса на покупку</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult CheckPayment(long buyingMyCryptRequestId)
    {
      BuyingMyCryptRequest buyingRequest = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<BuyingMyCryptRequest>()
        .Where(x => x.State == BuyingMyCryptRequestState.Accepted && x.Id == buyingMyCryptRequestId && x.Buyer.Id == CurrentSession.Default.CurrentUser.Id).FirstOrDefault();

      if (buyingRequest == null)
        throw new UserVisible__WrongParametrException("buyingMyCryptRequestId");

      Bill checkBill = buyingRequest.TradingSession.CheckBill;

      if (checkBill == null || checkBill.Payer.Id != CurrentSession.Default.CurrentUser.Id)
        throw new MLMExchange.Lib.Exception.ApplicationException(String.Format("Check bill for request {0} is not set or belong to anouther user", buyingRequest));

      if (checkBill.PaymentState == BillPaymentState.Paid)
        throw new UserVisible__CurrentActionAccessDenied();

      Payment checkPayment = new Payment
      {
        RealMoneyAmount = 120,
        Payer = CurrentSession.Default.CurrentUser
      };

      checkBill.Payments.Add(checkPayment);

      //TODO:Rtv Прикрепить платежную систему
      checkBill.PaymentState = BillPaymentState.Paid;

      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(buyingRequest);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }

    /// <summary>
    /// Оплатить комиссионый сбор пордавцу
    /// </summary>
    /// <param name="tradeSessionId">Id торговой сессии, по которой необходимо оплатить комиссионный взнос</param>
    /// <param name="paymentSystemGuid">Guid платежной системы, по которой осуществлялся платеж</param>
    /// <returns></returns>
    public ActionResult PaySallerInterestRate(long tradeSessionId, string paymentSystemGuid)
    {
      if (String.IsNullOrEmpty(paymentSystemGuid))
        throw new UserVisible__WrongParametrException("paymentSystemGuid");

      D_TradingSession tradingSession = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_TradingSession>()
        .Where(x => x.State == TradingSessionStatus.Open && x.Id == tradeSessionId).FirstOrDefault();

      if (tradingSession == null)
        throw new UserVisible__WrongParametrException("buyingMyCryptRequestId");

      if (tradingSession.BuyingMyCryptRequest.Buyer.Id != CurrentSession.Default.CurrentUser.Id)
        throw new UserVisible__CurrentActionAccessDenied();

      Bill sallerInterestRateBill = tradingSession.SallerInterestRateBill;

      if (sallerInterestRateBill == null || sallerInterestRateBill.Payer.Id != CurrentSession.Default.CurrentUser.Id)
        throw new MLMExchange.Lib.Exception.ApplicationException(String.Format("Saller interest pay bill for request {0} is not set or belong to anouther user", tradingSession));

      if (sallerInterestRateBill.PaymentState == BillPaymentState.Paid)
        throw new UserVisible__CurrentActionAccessDenied();

      PaymentSystem paymentSystem = Logic__PaymentSystem.GetPaymentSystemByGuid(paymentSystemGuid);

      if (paymentSystem == null)
        throw new UserVisible__WrongParametrException("paymentSystemGuid");

      //TODO:Rtv Прикрепить платежную систему
      Payment sallerInterestRatePayment = new Payment
      {
        RealMoneyAmount = 156,
        Payer = CurrentSession.Default.CurrentUser,
        PaymentSystem = paymentSystem
      };

      sallerInterestRateBill.Payments.Add(sallerInterestRatePayment);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }
  }
}
