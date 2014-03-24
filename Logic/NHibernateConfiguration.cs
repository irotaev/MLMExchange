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
        .ExposeConfiguration(cfg => new NHibernate.Tool.hbm2ddl.SchemaUpdate(cfg).Execute(true, true))
        .BuildSessionFactory();
    }

    public static ISessionFactory Session { get; private set; } 
  }
}
