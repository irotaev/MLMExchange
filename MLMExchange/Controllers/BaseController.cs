using Logic;
using MLMExchange.Models.Registration;
using MLMExchange.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Microsoft.Practices.Unity;

namespace MLMExchange.Controllers
{
  public abstract class BaseController : Controller
  {
    protected HtmlHelper _HtmlHelper;

    public BaseController()
    {
      //ViewData["AdminPanel__CenterBlock"] = 
    }

    protected override void Initialize(System.Web.Routing.RequestContext requestContext)
    {
      base.Initialize(requestContext);

      #region Инициализирую сессию NHibernate
      var session = Logic.NHibernateConfiguration.Session.OpenSession();
      session.BeginTransaction();

      MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.RegisterType<Logic.INHibernateManager, Logic.NHibernateManager>(new InjectionConstructor(session));
      #endregion

      #region Модель для логина
      LoginModel loginModel = new LoginModel();

      TryUpdateModel<LoginModel>(loginModel);

      ViewBag.LoginModel = loginModel;
      #endregion

      #region Добавляю данные в ViewData
      _HtmlHelper = new HtmlHelper(new ViewContext(ControllerContext, new WebFormView(ControllerContext, "FakeView"), new ViewDataDictionary(), new TempDataDictionary(), new System.IO.StringWriter()), new ViewPage());

      ViewData["AdminPanel__CenterBlock"] = _HtmlHelper.X().Panel().ID("AdminPanel__CenterBlock").Region(Region.Center);
      #endregion
    }

    protected override void Dispose(bool disposing)
    {
      #region Останавливаю сессию NHibernate
      var nhibernateManager = MLMExchange.Lib.ApplicationUnityContainer.UnityContainer.Resolve<Logic.INHibernateManager>();

      nhibernateManager.Session.Transaction.Commit();
      nhibernateManager.Session.Transaction.Dispose();
      nhibernateManager.Session.Dispose();
      #endregion

      base.Dispose(disposing);
    }

    protected override void OnException(ExceptionContext filterContext)
    {
      if (filterContext.ExceptionHandled)
        return;

      //filterContext.Result = new ViewResult
      //{
      //  ViewName = "~/Views/Shared/Error/Application__Error.cshtml",
      //  ViewData = new ViewDataDictionary(filterContext.Controller.ViewData)
      //  {
      //    Model = new MLMExchange.Models.Error.ApplicationExceptionModel { ExceptionMessage = filterContext.Exception.Message }
      //  }
      //};

      //filterContext.ExceptionHandled = true;
    }
  }
}
