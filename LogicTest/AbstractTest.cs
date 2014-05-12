using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate;

namespace LogicTest
{
  /// <summary>
  /// Базовый тест
  /// </summary>
  public abstract class AbstractTest
  {
    public AbstractTest()
    {
      #region Инициализирую сессию NHibernate
      Logic.NHibernateConfiguration.ConnectionString = "Data Source=IROTAEV-PC;Initial Catalog=mc_exchange;Integrated Security = SSPI;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";

      //TODO:Rtv переделать NHibernateConfiguration на static
      new Logic.NHibernateConfiguration();
      var session = NHibernateConfiguration.Session.OpenSession();
      session.BeginTransaction();

      Logic.Lib.ApplicationUnityContainer.UnityContainer.RegisterType<Logic.INHibernateManager, Logic.NHibernateManager>(new InjectionConstructor(session));
      #endregion

      _Session = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;
    }

    protected readonly ISession _Session;

    public static string NHibernateConnectionString { get; set; }

    /// <summary>
    /// Commit the transaction
    /// </summary>
    protected void TransactionCommit(bool isTransactionCommited = false)
    {
      #region Останавливаю сессию NHibernate
      var nhibernateManager = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<Logic.INHibernateManager>();
      
      if (!isTransactionCommited)
        nhibernateManager.Session.Transaction.Commit();

      nhibernateManager.Session.Transaction.Dispose();
      nhibernateManager.Session.Dispose();
      #endregion
    }
  }
}
