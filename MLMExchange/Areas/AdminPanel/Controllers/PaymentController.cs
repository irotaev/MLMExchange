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
}
