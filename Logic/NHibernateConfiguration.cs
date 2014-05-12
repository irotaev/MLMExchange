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
  public class NHibernateConfiguration
  {
    public NHibernateConfiguration()
    {
      if (Session == null)
      {
        var msSqlConfigurator = MsSqlConfiguration.MsSql2008.ShowSql();

        if (String.IsNullOrEmpty(ConnectionString))
          msSqlConfigurator.ConnectionString(c => c.FromConnectionStringWithKey("MsSQL2008"));
        else
          msSqlConfigurator.ConnectionString(ConnectionString);

        Session = Fluently.Configure()
          .Database(msSqlConfigurator)
          .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateConfiguration>())
          .ExposeConfiguration(cfg =>
          {
            cfg.EventListeners.PreInsertEventListeners = new NHibernate.Event.IPreInsertEventListener[] { new PreInsertEvent() };
            new NHibernate.Tool.hbm2ddl.SchemaUpdate(cfg).Execute(true, true);
          })
          .BuildSessionFactory();
      }
    }

    /// <summary>
    /// Строка соединения с базой данных.
    /// Если пуста, то берется из web.config
    /// </summary>
    public static string ConnectionString { get; set; }
    /// <summary>
    /// Текущая сессия
    /// </summary>
    public static ISessionFactory Session { get; private set; }
  }

  public interface INHibernateManager
  {
    ISession Session { get; }
  }

  public class NHibernateManager : INHibernateManager
  {
    public NHibernateManager(ISession session)
    {
      if (session == null)
        throw new ArgumentNullException("session");

      _Session = session;
    }

    private readonly ISession _Session;

    public ISession Session { get { return _Session; } }
  }
}
