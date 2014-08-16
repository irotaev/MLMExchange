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
using MLMExchange.Models;
using System.Web.Http;
using MLMExchange.Models.Error;
using Logic.Lib;
using Logic.Lib.Logging;

namespace MLMExchange.Controllers
{
  /// <summary>
  /// Интерфейс контроллера объекта данных
  /// <typeparam name="TDataModel">Тип модели объекта</typeparam>
  /// </summary>
  public interface IDataObjectController<in TDataModel>
    where TDataModel : class, IDataModel
  {
    /// <summary>
    /// Просмотр объекта данных
    /// </summary>
    /// <returns></returns>
    [System.Web.Mvc.HttpGet]
    ActionResult Browse();

    /// <summary>
    /// Редактировать объект данных
    /// </summary>
    /// <param name="model">Модель объекта данных</param>
    /// <returns></returns>
    ActionResult Edit(TDataModel model);
  }

  #region IDataObjectCustomizableController
  /// <summary>
  /// Интерфейс контроллера объекта данных. Поддерживает Кастомизацию экшенов
  /// <typeparam name="TDataModel">Тип модели объекта</typeparam>
  /// <typeparam name="TBrowseActionSettings">Настройки Action Browse</typeparam>
  /// <typeparam name="TEditActionSettings">Настройки Action Edit</typeparam>
  /// </summary>
  public interface IDataObjectCustomizableController<in TDataModel, in TBrowseActionSettings, in TEditActionSettings, in TListActionSettings>
    where TDataModel : class, IDataModel
    where TBrowseActionSettings : BaseBrowseActionSettings
    where TEditActionSettings : BaseEditActionSettings
    where TListActionSettings : BaseListActionSetings
  {
    /// <summary>
    /// Просмотр объекта данных
    /// </summary>
    /// <param name="actionSettings">Настройки принятые из Url</param>
    /// <returns></returns>
    [System.Web.Mvc.HttpGet]
    ActionResult Browse([FromUri] TBrowseActionSettings actionSettings);

    /// <summary>
    /// Редактировать объект данных
    /// </summary>
    /// <param name="model">Модель объекта данных</param>
    /// <param name="actionSettings">Настройки принятые из Url</param>
    /// <returns></returns>
    ActionResult Edit(TDataModel model, [FromUri] TEditActionSettings actionSettings);

    /// <summary>
    /// Вывести список объектов
    /// </summary>
    /// <param name="actionSettings">Настройки принятые из Url</param>
    /// <returns></returns>
    ActionResult List([FromUri] TListActionSettings actionSettings);
  }

  #region Базовые сущности для параметров акшенов
  /// <summary>
  /// Интерфейс частичного представления для вьюшки
  /// </summary>
  public interface IPartialView
  {
    bool? AsPartial { get; set; }
  }

  /// <summary>
  /// Базовые настройки Action Browse
  /// </summary>
  public class BaseBrowseActionSettings : IPartialView
  {
    /// <summary>
    /// Id объекта для отображения.
    /// Id уровня данных
    /// </summary>
    public long? objectId { get; set; }
    /// <summary>
    /// Отобразить ли как частичное представление
    /// </summary>
    public bool? AsPartial { get; set; }
  }

  /// <summary>
  /// Базовые настройки Action Browse
  /// </summary>
  public class BaseEditActionSettings : IPartialView
  {
    /// <summary>
    /// Id объекта для отображения.
    /// Id уровня данных
    /// </summary>
    public long? objectId { get; set; }
    /// <summary>
    /// Отобразить ли как частичное представление
    /// </summary>
    public bool? AsPartial { get; set; }
  }

  /// <summary>
  /// Базовые настройки Action List
  /// </summary>
  public class BaseListActionSetings : IPartialView
  {
    /// <summary>
    /// Список Id для отображения.
    /// Id уровня данных
    /// </summary>
    public IEnumerable<long> ObjectIds { get; set; }
    /// <summary>
    /// Отобразить ли как частичное представление
    /// </summary>
    public bool? AsPartial { get; set; }
  }
  #endregion

  #endregion

  public abstract class BaseController : Controller
  {
    protected HtmlHelper _HtmlHelper;
    /// <summary>
    /// Сессия NHibernate для текущего запроса
    /// </summary>
    protected NHibernate.ISession _NHibernateSession;

    /// <summary>
    /// Логгер NLog
    /// </summary>
    protected NLogLogger _NLogLogger = new NLogLogger();

    public BaseController()
    {
      //ViewData["AdminPanel__CenterBlock"] = 
    }

    public enum RedirectType : int
    {
      SuccessRegister = 0,
      SuccessResetPassword = 1,
      SuccessActivated = 3,
      SuccessSendMail = 4
    }

    protected override void Initialize(System.Web.Routing.RequestContext requestContext)
    {
      base.Initialize(requestContext);

      #region Инициализирую сессию NHibernate
      //TODO:Rtv переделать NHibernateConfiguration на static      
      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().TryOpenSession(SessionStorageType.ASPNET);
      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.BeginTransaction();

      _NHibernateSession = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;
      #endregion

      #region Модель для логина
      LoginModel loginModel = new LoginModel();

      TryUpdateModel<LoginModel>(loginModel);

      ViewBag.LoginModel = loginModel;
      #endregion

      #region Модель восстановления пароля
      ResetPasswordModel resetModel = new ResetPasswordModel();
      TryUpdateModel<ResetPasswordModel>(resetModel);

      ViewBag.ResetPasswordModel = resetModel;
      #endregion

      #region Добавляю данные в ViewData
      _HtmlHelper = new HtmlHelper(new ViewContext(ControllerContext, new WebFormView(ControllerContext, "FakeView"), new ViewDataDictionary(), new TempDataDictionary(), new System.IO.StringWriter()), new ViewPage());

      ViewData["AdminPanel__CenterBlock"] = _HtmlHelper.X().Panel().ID("AdminPanel__CenterBlock").Region(Region.Center);
      #endregion

      //TODO:Rtv Переработать это место
      //Logic.Application.Instance.Initiliaze();

      //if (Logic.Application.isTestBuild)
      //{
      //  ViewBag.isTestBuild = true;
      //  ViewBag.PeopleCounter = Logic.User.GetUserCountToStart(Logic.Application.UserCountToStart);
      //}
    }

    protected override void Dispose(bool disposing)
    {
      #region Останавливаю сессию NHibernate
      var nhibernateManager = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<Logic.INHibernateManager>();

      nhibernateManager.Session.Transaction.Commit();
      nhibernateManager.Session.Transaction.Dispose();
      nhibernateManager.Session.Dispose();
      #endregion

      base.Dispose(disposing);
    }

    protected override void OnException(ExceptionContext filterContext)
    {
      if (filterContext == null)
        throw new ArgumentNullException("filterContext");

      _NLogLogger.Fatal(filterContext.Exception);

      #if DEBUG
      if (filterContext.ExceptionHandled)
        return;
      #else
        if (!filterContext.ExceptionHandled)
        return;
      #endif

      #region Отображение ошибки
      if (!Request.IsAjaxRequest())
      {
      var model = new ApplicationExceptionModel();

      if (filterContext.Exception != null)
      {
        model.ExceptionMessage = filterContext.Exception.GetAllExceptionTreeLog("<br/><br/>");
      }
      
      System.Web.HttpContext.Current.ClearError();

      
        filterContext.Result = new ViewResult
        {
          ViewName = "~/Views/Shared/Exceptions/ApplicationException.cshtml",
          ViewData = new ViewDataDictionary(filterContext.Controller.ViewData) { Model = model }
        };
      }
      else
      {
        filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
        filterContext.Result = new JsonResult
        {
          JsonRequestBehavior = JsonRequestBehavior.AllowGet,
          Data = new
          {
            filterContext.Exception.Message
#if DEBUG
            ,filterContext.Exception.StackTrace
#endif
          }
        };
      }

      //PartialView("~/Views/Shared/Exceptions/ApplicationException.cshtml", model).ExecuteResult(this.ControllerContext);
      #endregion

      filterContext.ExceptionHandled = true;
      filterContext.HttpContext.Response.Clear();
      filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
    }
  }
}
