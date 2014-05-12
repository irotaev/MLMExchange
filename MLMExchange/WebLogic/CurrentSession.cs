using L = Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Logic;
using System.Web.SessionState;
using Microsoft.Practices.Unity;

namespace MLMExchange.Lib
{
  /// <summary>
  /// Текущая сессия. Потокобезопасна
  /// </summary>
  public sealed class CurrentSession
  {
    private CurrentSession(HttpSessionState session)
    {
      if (session == null)
        throw new ApplicationException("session is null");

      _Session = session;
    }

    /// <summary>
    /// Объект для многопоточной блокировки объектов
    /// </summary>
    private static readonly object _LockerObject = new object();
    private readonly HttpSessionState _Session;
    private L.D_User _CurrentUser;

    /// <summary>
    /// Хранилище сессий для каждого SessionId
    /// </summary>
    private readonly static Dictionary<string, CurrentSession> _SessionStorage = new Dictionary<string, CurrentSession>();

    public static CurrentSession Default
    {
      get
      {
        if (String.IsNullOrEmpty(HttpContext.Current.Session.SessionID))
          return null;

        lock (_LockerObject)
        {
          return new CurrentSession(HttpContext.Current.Session);
          //if (_SessionStorage.Keys.Contains(HttpContext.Current.Session.SessionID))
          //{
          //  return _SessionStorage[HttpContext.Current.Session.SessionID];
          //}
          //else
          //{
          //  CurrentSession currentSession = new CurrentSession(HttpContext.Current.Session);
          //  _SessionStorage.Add(HttpContext.Current.Session.SessionID, currentSession);

          //  return currentSession;               
          //}
        }
      }
    }

    public L.D_User CurrentUser
    {
      get
      {
        if (HttpContext.Current.Request.Cookies["_AUTHORIZE"] == null)
          return null;

        if (_Session["Login"] == null)
          return null;

        if (!String.IsNullOrEmpty(_Session["Login"].ToString()) && _CurrentUser == null)
        {
          L.D_User findUser = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
            .QueryOver<L.D_User>().List().FirstOrDefault(u => u.Login == (string)_Session["Login"]);

          if (findUser != null)
            _CurrentUser = findUser;

          _Session["IsNeedUpdateCurrentInfo"] = false;
        }

        return _CurrentUser;
      }
    }
  }
}