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

  /// <summary>
  /// Базовые настройки Action Browse
  /// </summary>
  public class BaseBrowseActionSettings 
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
  public class BaseEditActionSettings { }

  /// <summary>
  /// Базовые настройки Action List
  /// </summary>
  public class BaseListActionSetings {}
  #endregion

  public abstract class BaseController : Controller
  {
    protected HtmlHelper _HtmlHelper;
    /// <summary>
    /// Сессия NHibernate для текущего запроса
    /// </summary>
    protected NHibernate.ISession _NHibernateSession;

    public BaseController()
    {
      //ViewData["AdminPanel__CenterBlock"] = 
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

      #region Добавляю данные в ViewData
      _HtmlHelper = new HtmlHelper(new ViewContext(ControllerContext, new WebFormView(ControllerContext, "FakeView"), new ViewDataDictionary(), new TempDataDictionary(), new System.IO.StringWriter()), new ViewPage());

      ViewData["AdminPanel__CenterBlock"] = _HtmlHelper.X().Panel().ID("AdminPanel__CenterBlock").Region(Region.Center);
      #endregion

      //TODO:Rtv Переработать это место
      Logic.Application.Instance.Initiliaze();
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
