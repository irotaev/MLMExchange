using MLMExchange.Areas.AdminPanel.Models.PaymentSystem;
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
using Logic;

namespace MLMExchange.Areas.AdminPanel.Controllers.PaymentSystems
{
  /// <summary>
  /// Контроллер платёжной системы типа электронная платёжная система
  /// </summary>
  public class ElectronicPaymentSystemController : BaseController, IDataObjectController<ElectronicPaymentSystemModel>
  {
    public System.Web.Mvc.ActionResult Browse()
    {
      throw new NotImplementedException();
    }

    public System.Web.Mvc.ActionResult Edit(ElectronicPaymentSystemModel model)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Удалить платёжную сисетму типа электронная платёжная система
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult Remove(long id)
    {
      D_ElectronicPaymentSystem electronicPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_ElectronicPaymentSystem>().Where(x => x.PaymentSystemGroup.User.Id == CurrentSession.Default.CurrentUser.Id && x.Id == id).FirstOrDefault();

      if (electronicPaymentSystem == null)
        throw new UserVisible__WrongParametrException("id");

      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Delete(electronicPaymentSystem);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }

    public ActionResult SetDefault(long id)
    {
      D_ElectronicPaymentSystem electronicPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_ElectronicPaymentSystem>().Where(x => x.PaymentSystemGroup.User.Id == CurrentSession.Default.CurrentUser.Id && x.Id == id).FirstOrDefault();

      if (electronicPaymentSystem == null)
        throw new UserVisible__WrongParametrException("id");

      if (electronicPaymentSystem.IsDefault)
        throw new UserVisibleException(MLMExchange.Properties.PrivateResource.Exception_SystemAlreadyDefault);

      #region D_ElectronicPaymentSystem
      D_ElectronicPaymentSystem electronicDefaultPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_ElectronicPaymentSystem>().Where(x => x.PaymentSystemGroup.User.Id == CurrentSession.Default.CurrentUser.Id && x.IsDefault == true).FirstOrDefault();
      #endregion

      #region D_BankPaymentSystem
      D_BankPaymentSystem bankDefaultPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_BankPaymentSystem>().Where(x => x.PaymentSystemGroup.User.Id == CurrentSession.Default.CurrentUser.Id && x.IsDefault == true).FirstOrDefault();
      #endregion

      if (electronicDefaultPaymentSystem != null)
      {
        electronicDefaultPaymentSystem.IsDefault = false;
        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(electronicDefaultPaymentSystem);
      }
      if (bankDefaultPaymentSystem != null)
      {
        bankDefaultPaymentSystem.IsDefault = false;
        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(bankDefaultPaymentSystem);
      }

      electronicPaymentSystem.IsDefault = true;
      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(electronicPaymentSystem);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }
  }
  
}
