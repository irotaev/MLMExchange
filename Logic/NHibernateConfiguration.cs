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
    static NHibernateConfiguration()
    {
      Session = Fluently.Configure()
        .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromConnectionStringWithKey("MsSQL2008")).ShowSql())
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateConfiguration>())
        .ExposeConfiguration(cfg =>
        {
          cfg.EventListeners.PreInsertEventListeners = new NHibernate.Event.IPreInsertEventListener[] { new PreInsertEvent() };
          new NHibernate.Tool.hbm2ddl.SchemaUpdate(cfg).Execute(true, true);
        })
        .BuildSessionFactory();
    }

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
