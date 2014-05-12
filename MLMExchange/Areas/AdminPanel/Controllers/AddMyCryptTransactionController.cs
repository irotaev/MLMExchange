using MLMExchange.Areas.AdminPanel.Models.User;
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
  [Auth(typeof(D_AdministratorRole))]
  public class AddMyCryptTransactionController : BaseController, IDataObjectCustomizableController<AddMyCryptModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    /// <summary>
    /// Список неподтвержденых транзакций
    /// </summary>
    /// <returns></returns>
    public ActionResult UnApprovedTransactionList()
    {
      IEnumerable<AddMyCryptModel> model = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_AddMyCryptTransaction>().Where(x => x.State == AddMyCryptTransactionState.NA).Select(x => new AddMyCryptModel().Bind(x));

      return View(model);
    }

    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      if (actionSettings.objectId == null)
        throw new UserVisible__ArgumentNullException("objectId");

      D_AddMyCryptTransaction transaction = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_AddMyCryptTransaction>().Where(x => x.Id == actionSettings.objectId).FirstOrDefault();

      if (transaction == null)
        throw new UserVisible__WrongParametrException("objectId");

      if (actionSettings.AsPartial != null)
      {
        if (actionSettings.AsPartial.Value)
          ViewData["AsPartial"] = "True";
      }

      return View(new AddMyCryptModel().Bind(transaction));
    }

    public ActionResult Edit(AddMyCryptModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Подтвержить запрос на получение my-crypt
    /// </summary>
    /// <param name="transactionId">Id транзакции</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult AcceptTransaction(long transactionId)
    {
      var session = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;

      D_AddMyCryptTransaction d_Transaction = session.Query<D_AddMyCryptTransaction>().Where(x => x.Id == transactionId).FirstOrDefault();

      if (d_Transaction == null)
        throw new UserVisible__WrongParametrException("transactionId");

      D_UserRole userRole = d_Transaction.User.Roles.Where(x => x.RoleType == RoleType.User).FirstOrDefault() as D_UserRole;

      if (userRole == null)
        throw new UserVisible__CurrentActionAccessDenied();

      userRole.MyCryptCount += d_Transaction.MyCryptCount;

      d_Transaction.State = AddMyCryptTransactionState.Approved;

      session.SaveOrUpdate(d_Transaction);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }

    /// <summary>
    /// Отклонить запрос на получение my-crypt
    /// </summary>
    /// <param name="transactionId">Id транзакции</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult DeniedTransaction(long transactionId)
    {
      var session = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;

      D_AddMyCryptTransaction d_Transaction = session.Query<D_AddMyCryptTransaction>().Where(x => x.Id == transactionId).FirstOrDefault();

      if (d_Transaction == null)
        throw new UserVisible__WrongParametrException("transactionId");

      d_Transaction.State = AddMyCryptTransactionState.NotApproved;

      session.SaveOrUpdate(d_Transaction);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }
  }
}
