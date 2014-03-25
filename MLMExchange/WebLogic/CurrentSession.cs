using L = Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Providers.Entities;
using Logic;
using System.Web.SessionState;

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
    private L.User _CurrentUser;

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

    public L.User CurrentUser
    {
      get
      {
        if (HttpContext.Current.Request.Cookies["_AUTHORIZE"] == null)
          return null;

        if (_Session["Login"] == null)
          return null;

        if (!String.IsNullOrEmpty(_Session["Login"].ToString()) && _CurrentUser == null)
        {
          using (var session = NHibernateConfiguration.Session.OpenSession())
          {
            using (var transaction = session.BeginTransaction())
            {
              L.User findUser = session.QueryOver<L.User>().List().FirstOrDefault(u => u.Login == (string)_Session["Login"]);

              if (findUser != null)
                _CurrentUser = findUser;
            }
          }

          _Session["IsNeedUpdateCurrentInfo"] = false;
        }

        return _CurrentUser;
      }
    }
  } 
}