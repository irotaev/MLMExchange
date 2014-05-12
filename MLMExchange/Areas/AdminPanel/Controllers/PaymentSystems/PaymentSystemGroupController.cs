using MLMExchange.Areas.AdminPanel.Models.PaymentSystem;
using MLMExchange.Controllers;
using MLMExchange.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Logic;
using NHibernate.Linq;
using MLMExchange.Lib.Exception;
using MLMExchange.Areas.AdminPanel.Views.PaymentSystemGroup;

namespace MLMExchange.Areas.AdminPanel.Controllers.PaymentSystems
{
  [Auth]
  public class PaymentSystemGroupController : BaseController, IDataObjectCustomizableController<PaymentSystemGroupModel, BrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public ActionResult Browse(BrowseActionSettings actionSettings)
    {
      NHibernate.ISession session = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;

      PaymentSystemGroup_Browse__ViewSetting viewSettings = new PaymentSystemGroup_Browse__ViewSetting
      {
        IsEnableAddPaymentSystem = true,
        IsHeaderEnable = true,
        IsPaymentSystemOperationRootOperationEnable = true,
      };

      PaymentSystemGroup paymentSystemGroup;

      if (actionSettings.ForUserId == null)
      {
        paymentSystemGroup = session.Query<D_User>().Where(x => x.Id == CurrentSession.Default.CurrentUser.Id).Select(x => x.PaymentSystemGroup).FirstOrDefault();

        viewSettings.IsRootUser = true;
      }
      else
      {
        paymentSystemGroup = session.Query<PaymentSystemGroup>().Where(x => x.User.Id == actionSettings.ForUserId).FirstOrDefault();

        viewSettings.IsRootUser = false;
      }

      PaymentSystemGroupModel model = new PaymentSystemGroupModel().Bind(paymentSystemGroup);
      
      if (paymentSystemGroup == null)
        throw new MLMExchange.Lib.Exception.ApplicationException("This user has no payment system group");

      if (actionSettings.IsRequireSallerInterestRatePayment != null)
      {
        if (actionSettings.IsRequireSallerInterestRatePayment.Value == true)
        {
          if (actionSettings.TradeSessionId == null)
            throw new UserVisible__WrongParametrException("TradeSessionId");

          D_TradingSession tradingSession = session.Query<D_TradingSession>()
            .Where(x => x.Id == actionSettings.TradeSessionId.Value && x.State != TradingSessionStatus.Closed && x.State != TradingSessionStatus.NA).FirstOrDefault();

          if (tradingSession == null)
            throw new UserVisible__WrongParametrException("PaymentSystemGroupId");

          if (tradingSession.BuyingMyCryptRequest.Buyer.Id != CurrentSession.Default.CurrentUser.Id)
            throw new UserVisible__CurrentActionAccessDenied();

          model.PaySellerInterestRateModel__TradeSessionId = actionSettings.TradeSessionId.Value;
        }

        viewSettings.IsRequireSallerInterestRatePayment = actionSettings.IsRequireSallerInterestRatePayment.Value;
      }

      var system = paymentSystemGroup.BankPaymentSystems.FirstOrDefault();

      ViewData["PaymentSystemGroup_Browse__ViewSetting"] = viewSettings;

      if (Request.IsAjaxRequest())
        ViewData["AsPartial"] = "True";

      return View(model);
    }

    public ActionResult Edit(PaymentSystemGroupModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Добавить панк в группу платежных систем текущего пользователя
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult AddBank(BankPaymentSystemModel model)
    {
      ModelState.Clear();

      TryUpdateModel<BankPaymentSystemModel>(model);

      if (ModelState.IsValid)
      {
        PaymentSystemGroup paymentSystemGroup = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_User>().Where(x => x.Id == CurrentSession.Default.CurrentUser.Id).Select(x => x.PaymentSystemGroup).FirstOrDefault();

        if (paymentSystemGroup == null)
          throw new MLMExchange.Lib.Exception.ApplicationException("This user has no payment system group");

        BankPaymentSystem bankPaymentSystem = model.UnBind();
        bankPaymentSystem.PaymentSystemGroup = paymentSystemGroup;

        paymentSystemGroup.BankPaymentSystems.Add(bankPaymentSystem);

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(paymentSystemGroup);
      }
      else
      {
        throw new UserVisible__WrongParametrException("model");
      }

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }


    public ActionResult List(BaseListActionSetings actionSettings)
    {
      throw new NotImplementedException();
    }
  }

  public class BrowseActionSettings : BaseBrowseActionSettings
  {
    /// <summary>
    /// Id пользователя, для которого необходимо отобразить группу
    /// </summary>
    public long? ForUserId { get; set; }

    #region Позволить оплатить комиссионный сбор для продавца
    /// <summary>-
    /// Позволить ли оплатить комиссионный сбор для продавца
    /// </summary>
    public bool? IsRequireSallerInterestRatePayment { get; set; }
    /// <summary>
    /// Id торговой сессии, для которой необходимо оплатить комиссионный сбор продавца
    /// </summary>
    public long? TradeSessionId { get; set; }
    #endregion
  }
}
