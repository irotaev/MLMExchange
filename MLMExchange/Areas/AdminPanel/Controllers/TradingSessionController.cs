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
    /// <returns></returns>
    [HttpPost]
    public ActionResult PayYieldTradingSessionBill(long yieldSessionBillId)
    {
      var session = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;

      D_YieldSessionBill d_yieldSessionBill = session.Query<D_YieldSessionBill>().Where(x => x.Id == yieldSessionBillId).FirstOrDefault();

      if (d_yieldSessionBill == null)
        throw new UserVisible__WrongParametrException("yieldSessionBillId");

      TradingSession tradingSession = (TradingSession)d_yieldSessionBill.TradingSession;

      if (d_yieldSessionBill.PaymentState != BillPaymentState.NA 
        || d_yieldSessionBill.Payer.Id != CurrentSession.Default.CurrentUser.Id 
        || tradingSession.LogicObject.State != TradingSessionStatus.Open)
      {
        throw new UserVisible__CurrentActionAccessDenied();
      }

      Payment payment = new Payment
      {
        Payer = CurrentSession.Default.CurrentUser,
        
      }
    }
  }
}