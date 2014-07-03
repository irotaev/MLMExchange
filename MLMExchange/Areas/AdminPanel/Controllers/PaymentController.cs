using Logic;
using MLMExchange.Areas.AdminPanel.Models;
using MLMExchange.Controllers;
using MLMExchange.Lib;
using MLMExchange.Lib.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  public class BasePaymentController : BaseController, 
    IDataObjectCustomizableController<BasePaymentModel, BaseBrowseActionSettings, BaseEditActionSettings,      MLMExchange.Areas.AdminPanel.Controllers.BasePaymentController.CustomListActionSettings>
  {

    public class CustomListActionSettings : BaseListActionSetings 
    {
      /// <summary>
      /// Id платежных систем
      /// </summary>
      public IEnumerable<long> PaymentSystemIds { get; set; }
    }

    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult Edit(BasePaymentModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    [HttpGet]
    public ActionResult List(BasePaymentController.CustomListActionSettings actionSettings)
    {
      if (actionSettings.AsPartial != null)
        ViewData["AsPartial"] = actionSettings.AsPartial.Value ? "True" : "False";

      List<IBasePaymentModel> model = new List<IBasePaymentModel>();

      foreach(var id in actionSettings.PaymentSystemIds)
      {
        Payment d_payment = _NHibernateSession.Query<Payment>().Where(x => x.Id == id).FirstOrDefault();

        if (d_payment == null)
          throw new UserVisible__WrongParametrException("Payment id");

        if (d_payment.PaymentSystem == null)
          continue;

        d_payment.PaymentSystem = ((PaymentSystem)d_payment.PaymentSystem).Load().LogicObject ?? d_payment.PaymentSystem;

        if (d_payment.PaymentSystem is D_BankPaymentSystem)
        {
          model.Add(new BankPaymentModel().Bind(d_payment));
        }
        else if (d_payment.PaymentSystem is D_ElectronicPaymentSystem)
        {
          model.Add(new ElectronicPaymentModel().Bind(d_payment));
        }
      }

      return View(model);
    }
  }

  /// <summary>
  /// Контроллер платежа банка
  /// </summary>
  [Auth(typeof(D_UserRole))]
  public class BankPaymentController : BaseController, IDataObjectCustomizableController<BankPaymentModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      if (actionSettings.objectId == null)
        throw new UserVisible__ArgumentNullException("objectId");

      Payment payment = _NHibernateSession.Query<Payment>().Where(x => x.Id == actionSettings.objectId.Value).FirstOrDefault();

      if (payment == null)
        throw new UserVisible__WrongParametrException("objectId");

      BankPaymentModel paymentModel = new BankPaymentModel().Bind(payment);

      if (Request.IsAjaxRequest())
        ViewData["AsPartial"] = "True";

      return View(paymentModel);
    }

    public ActionResult Edit(BankPaymentModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Контроллер платёжа электронной платёжной системы
  /// </summary>
  [Auth(typeof(D_UserRole))]
  public class ElectronicPaymentController : BaseController, IDataObjectCustomizableController<ElectronicPaymentModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      if (actionSettings.objectId == null)
        throw new UserVisible__ArgumentNullException("objectId");

      Payment payment = _NHibernateSession.Query<Payment>().Where(x => x.Id == actionSettings.objectId.Value).FirstOrDefault();

      if (payment == null)
        throw new UserVisible__WrongParametrException("objectId");

      ElectronicPaymentModel electronicModel = new ElectronicPaymentModel().Bind(payment);

      if (Request.IsAjaxRequest())
        ViewData["AsPartial"] = "True";

      return View(electronicModel);
    }

    public ActionResult Edit(ElectronicPaymentModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      throw new NotImplementedException();
    }
  }

}
