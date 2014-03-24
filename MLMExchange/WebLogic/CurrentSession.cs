using L = Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Providers.Entities;
using Logic;

namespace MLMExchange.Lib
{
  /// <summary>
  /// Текущая сессия
  /// </summary>
  public class CurrentSession
  {
    public CurrentSession(HttpSessionStateBase session)
    {
      if (session == null)
        throw new ApplicationException("session is null");

      _Session = session;
    }

    private readonly HttpSessionStateBase _Session;

    private L.User _CurrentUser;
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