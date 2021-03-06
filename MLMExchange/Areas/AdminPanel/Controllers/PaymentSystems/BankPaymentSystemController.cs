﻿using MLMExchange.Areas.AdminPanel.Models.PaymentSystem;
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
using Logic.Lib;

namespace MLMExchange.Areas.AdminPanel.Controllers.PaymentSystems
{
  /// <summary>
  /// Контроллер платежной системы типа банк. 
  /// Операции с системой данного вида
  /// </summary>
  [Auth]
  public class BankPaymentSystemController : BaseController, IDataObjectController<BankPaymentSystemModel>
  {
    public System.Web.Mvc.ActionResult Browse()
    {
      throw new NotImplementedException();
    }

    public System.Web.Mvc.ActionResult Edit(BankPaymentSystemModel model)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Удалить платежную систему типа банк
    /// </summary>
    /// <param name="id">Id платежной системы</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult Remove(long id)
    {
      D_BankPaymentSystem bankPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_BankPaymentSystem>().Where(x => x.PaymentSystemGroup.User.Id == CurrentSession.Default.CurrentUser.Id && x.Id == id).FirstOrDefault();

      if (bankPaymentSystem == null)
        throw new UserVisible__WrongParametrException("id");

      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Delete(bankPaymentSystem);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }

    /// <summary>
    /// Сделать систему по-умолчанию
    /// </summary>
    /// <param name="id">Id системы</param>
    /// <returns></returns>
    public ActionResult SetDefault(long id)
    {
      D_BankPaymentSystem bankPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_BankPaymentSystem>().Where(x => x.PaymentSystemGroup.User.Id == CurrentSession.Default.CurrentUser.Id && x.Id == id).FirstOrDefault();

      if (bankPaymentSystem == null)
        throw new UserVisible__WrongParametrException("id");

      if (bankPaymentSystem.IsDefault)
        throw new Logic.Lib.UserVisibleException(MLMExchange.Properties.PrivateResource.Exception_SystemAlreadyDefault);

      #region D_ElectronicPaymentSystem
      D_ElectronicPaymentSystem electronicDefaultPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_ElectronicPaymentSystem>().Where(x => x.PaymentSystemGroup.User.Id == CurrentSession.Default.CurrentUser.Id && x.IsDefault == true).FirstOrDefault();
      #endregion

      #region D_BankPaymentSystem
      D_BankPaymentSystem bankDefaultPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_BankPaymentSystem>().Where(x => x.PaymentSystemGroup.User.Id == CurrentSession.Default.CurrentUser.Id && x.IsDefault == true).FirstOrDefault();
      #endregion

      if (bankDefaultPaymentSystem != null)
      {
        bankDefaultPaymentSystem.IsDefault = false;
        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(bankDefaultPaymentSystem);
      }
      if (electronicDefaultPaymentSystem != null)
      {
        electronicDefaultPaymentSystem.IsDefault = false;
        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(electronicDefaultPaymentSystem);
      }

      bankPaymentSystem.IsDefault = true;
      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.SaveOrUpdate(bankPaymentSystem);

      if (!Request.IsAjaxRequest())
        return Redirect(Request.UrlReferrer.ToString());
      else
        return null;
    }
  }
}