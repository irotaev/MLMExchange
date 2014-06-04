using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Logic
{
  /// <summary>
  /// Конфигурация NHibernate.
  /// Синглтон
  /// </summary>
  public sealed class NHibernateConfiguration
  {
    private NHibernateConfiguration()
    {
      var msSqlConfigurator = MsSqlConfiguration.MsSql2008.ShowSql();

      if (String.IsNullOrEmpty(ConnectionString))
        msSqlConfigurator.ConnectionString(c => c.FromConnectionStringWithKey("MsSQL2008"));
      else
        msSqlConfigurator.ConnectionString(ConnectionString);

      _SessionFactory = Fluently.Configure()
        .Database(msSqlConfigurator)
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateConfiguration>())
        .ExposeConfiguration(cfg =>
        {
          cfg.EventListeners.PreInsertEventListeners = new NHibernate.Event.IPreInsertEventListener[] { new PreInsertEvent() };
          cfg.EventListeners.PreUpdateEventListeners = new NHibernate.Event.IPreUpdateEventListener[] { new PreUpdateEvent() };
          new NHibernate.Tool.hbm2ddl.SchemaUpdate(cfg).Execute(true, true);
        })
        .BuildSessionFactory();
    }

    /// <summary>
    /// Экземпляр класса.
    /// Синглтон
    /// </summary>
    private static NHibernateConfiguration _Instance;

    [ThreadStatic]
    private static ISession _CurrentSession_ThreadStatic;

    private const string _HttpContextSessionKey = "NHibernateSession";

    /// <summary>
    /// Текущая фабрика сессия
    /// </summary>
    private readonly ISessionFactory _SessionFactory;


    /// <summary>
    /// Получить экземпляр класса. Синглтон
    /// </summary>
    public static NHibernateConfiguration Instance
    {
      get
      {
        if (_Instance == null)
          _Instance = new NHibernateConfiguration();

        return _Instance;
      }
    }

    /// <summary>
    /// Строка соединения с базой данных.
    /// Если пуста, то берется из web.config
    /// </summary>
    public static string ConnectionString { get; set; }

    /// <summary>
    /// Текущая открытая сессия.
    /// ThreadStatic (Уникальная для каждого потока)
    /// </summary>
    public static ISession CurrentSession_ThreadStatic { get { return _CurrentSession_ThreadStatic; } }
    public static ISession CurrentSession_ASPNET
    {
      get
      {

        if (System.Web.HttpContext.Current != null)
        {
          return (ISession)System.Web.HttpContext.Current.Items[_HttpContextSessionKey];
        }
        else
        {
          return null;
        }
      }
    }

    /// <summary>
    /// Открыть сессию NHibernate
    /// </summary>
    /// <param name="storageType">Тип хранилища для сессии</param>
    /// <returns>Открыта ли новая сессия - true, или сессия уже существует</returns>
    public bool TryOpenSession(SessionStorageType storageType)
    {
      bool result = false;
      ISession createdSession = null;

      if (_CurrentSession_ThreadStatic == null)
      {
        createdSession = _SessionFactory.OpenSession();
        _CurrentSession_ThreadStatic = createdSession;

        result = true;
      }

      switch (storageType)
      {
        case SessionStorageType.ASPNET:
          if (createdSession == null)
            createdSession = _SessionFactory.OpenSession();

          if (!System.Web.HttpContext.Current.Items.Contains(_HttpContextSessionKey))
          {
            System.Web.HttpContext.Current.Items.Add(_HttpContextSessionKey, createdSession);
            result = true;
          }
          break;
      }

      return result;
    }
  }

  /// <summary>
  /// Принцип хранения сессии
  /// </summary>
  public enum SessionStorageType
  {
    /// <summary>
    /// Сессия хранится в HttpContext.Items.
    /// Уникальна для каждого запроса
    /// </summary>
    ASPNET = 0,
    /// <summary>
    /// Сессия уникальна для каждого потока.
    /// </summary>
    ThreadStatic = 1
  }

  /// <summary>
  /// Интерфейс управления NHibernate.
  /// Реализуется Ioc контайнером
  /// </summary>
  public interface INHibernateManager
  {
    ISession Session { get; }

    /// <summary>
    /// Открыть сессию NHibernate
    /// </summary>
    /// <param name="storageType">Тип хранилища для сессии</param>
    /// <returns>Открыта ли новая сессия - true, или сессия уже существует</returns>
    bool TryOpenSession(SessionStorageType storageType);
  }

  /// <summary>
  /// Менеджер сессий NHibernate
  /// </summary>
  public class NHibernateManager : INHibernateManager
  {
    public NHibernateManager() { }
    
    public bool TryOpenSession(SessionStorageType storageType)
    {
      return NHibernateConfiguration.Instance.TryOpenSession(storageType);
    }

    /// <summary>
    /// Текущая сессия для конкретного экземпляра класса. 
    /// Например, уникальная для потока или для ASP.NET сессии
    /// </summary>
    public ISession Session
    {
      get
      {
        if (NHibernateConfiguration.CurrentSession_ASPNET != null)
        {
          return NHibernateConfiguration.CurrentSession_ASPNET;
        }
        else if (NHibernateConfiguration.CurrentSession_ThreadStatic != null)
        {
          return NHibernateConfiguration.CurrentSession_ThreadStatic;
        }

        return null;
      }
    }
  }
}
