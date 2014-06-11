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
  public class TradingSessionController : BaseController, IDataObjectCustomizableController<TradingSessionModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public System.Web.Mvc.ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }
    
    public System.Web.Mvc.ActionResult Edit(TradingSessionModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public System.Web.Mvc.ActionResult List(BaseListActionSetings actionSettings)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Оплатить счет на доходность торговой сессии
    /// </summary>
    /// <param name="paymentSystemId">Id платежной системы</param>
    /// <param name="yieldSessionBillId">Id счета на обеспечение доходности торговой сессии</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult PayYieldTradingSessionBill(long yieldSessionBillId, long paymentSystemId)
    {
      var session = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;

      D_YieldSessionBill d_yieldSessionBill = session.Query<D_YieldSessionBill>().Where(x => x.Id == yieldSessionBillId).FirstOrDefault();

      if (d_yieldSessionBill == null)
        throw new UserVisible__WrongParametrException("yieldSessionBillId");

      TradingSession tradingSession = (TradingSession)d_yieldSessionBill.TradingSession;

      if (d_yieldSessionBill.PaymentState != BillPaymentState.NA 
        || d_yieldSessionBill.Payer.Id != CurrentSession.Default.CurrentUser.Id 
        || tradingSession.LogicObject.State != TradingSessionStatus.NeedEnsureProfibility)
      {
        throw new UserVisible__CurrentActionAccessDenied();
      }

      PaymentSystem d_paymentSystem = session.Query<PaymentSystem>()
        .Where(x => x.Id == paymentSystemId).FirstOrDefault();

      if (d_paymentSystem == null)
        throw new UserVisible__WrongParametrException("paymentSystemId");

      Payment payment = new Payment
      {
        Payer = CurrentSession.Default.CurrentUser,
        PaymentSystem = d_paymentSystem,
        RealMoneyAmount = d_yieldSessionBill.MoneyAmount
      };
      
      d_yieldSessionBill.PaymentState = BillPaymentState.Paid;
      ((YieldSessionBill)d_yieldSessionBill).AddPayment(payment);

      session.SaveOrUpdate(d_yieldSessionBill);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }

    /// <summary>
    /// Перевести сессию в состояние "исполняется"
    /// </summary>
    /// <param name="tradingSessionId">Id торговой сессии</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult SetSessionInProgress(long tradingSessionId)
    {
      if (!Request.IsAjaxRequest())
        return null;

      var session = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;

      D_TradingSession tradingSession = session.Query<D_TradingSession>().Where(s => s.Id == tradingSessionId).FirstOrDefault();

      if (tradingSessionId == null)
        throw new UserVisible__ArgumentNullException("tradingSessionId");

      if (tradingSession.BiddingParticipateApplication.Seller.Id != CurrentSession.Default.CurrentUser.Id || tradingSession.State != TradingSessionStatus.WaitForProgressStart)
        throw new UserVisible__CurrentActionAccessDenied();

      ((TradingSession)tradingSession).TryChangeStatus(TradingSessionStatus.SessionInProgress);

      session.SaveOrUpdate(tradingSession);

      return null;
    }
  }
}