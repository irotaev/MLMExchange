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

      TradingSession tradingSession = (TradingSession)d_yieldSessionBill.PayerTradingSession;

      if (d_yieldSessionBill.PaymentState != BillPaymentState.NA 
        || d_yieldSessionBill.Payer.Id != CurrentSession.Default.CurrentUser.Id 
        || tradingSession.LogicObject.State != TradingSessionStatus.NeedEnsureProfibility)
      {
        throw new UserVisible__CurrentActionAccessDenied();
      }

      D_PaymentSystem d_paymentSystem = session.Query<D_PaymentSystem>()
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

      if (tradingSession == null)
        throw new UserVisible__ArgumentNullException("tradingSessionId");

      if (tradingSession.BuyingMyCryptRequest.Buyer.Id != CurrentSession.Default.CurrentUser.Id || tradingSession.State != TradingSessionStatus.WaitForProgressStart)
        throw new UserVisible__CurrentActionAccessDenied();

      ((TradingSession)tradingSession).TryChangeStatus(TradingSessionStatus.SessionInProgress);

      session.SaveOrUpdate(tradingSession);

      return null;
    }

    /// <summary>
    /// Закрыть торговую сессию
    /// </summary>
    /// <param name="tradingSessionId">Id торговой сессии</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult CloseTradingSession(long tradingSessionId)
    {
      if (!Request.IsAjaxRequest())
        throw new UserVisible__ActionAjaxOnlyException();

      D_TradingSession d_tradingSession = _NHibernateSession.Query<D_TradingSession>().Where(x => x.Id == tradingSessionId).FirstOrDefault();

      if (d_tradingSession == null)
        throw new UserVisible__WrongParametrException("tradingSessionId");

      if (!((TradingSession)d_tradingSession).TryChangeStatus(TradingSessionStatus.NeedProfit))
        throw new UserVisibleException(MLMExchange.Properties.PrivateResource.TradingSession_Close__CantClose);

      return null;
    }

    /// <summary>
    /// Подтвердить оплату по счету прибыли торговой сессии
    /// </summary>
    /// <param name="billId">Id счета</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult ConfirmNeedProfitBill(long billId)
    {
      if (!Request.IsAjaxRequest())
        throw new UserVisible__ActionAjaxOnlyException();

      D_YieldSessionBill d_bill = _NHibernateSession.Query<D_YieldSessionBill>().Where(x => x.Id == billId).FirstOrDefault();

      if (d_bill == null)
        throw new UserVisible__WrongParametrException("billId");

      if (d_bill.PaymentState != BillPaymentState.WaitingPayment || d_bill.PaymentAcceptor.Id != CurrentSession.Default.CurrentUser.Id)
        throw new UserVisible__CurrentActionAccessDenied();

      d_bill.PaymentState = BillPaymentState.Paid;
      ((TradingSession)d_bill.AcceptorTradingSession).TryChangeStatus(TradingSessionStatus.Closed);

      _NHibernateSession.SaveOrUpdate(d_bill);

      return null;
    }
  }
}