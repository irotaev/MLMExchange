using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicTest
{
  /// <summary>
  /// Базовый тест
  /// </summary>
  [TestClass]
  public abstract class AbstractTest
  {
    static AbstractTest()
    {
      Logic.Lib.ApplicationUnityContainer.UnityContainer.RegisterType<Logic.INHibernateManager, Logic.NHibernateManager>();
    }

    public AbstractTest()
    {
      #region Инициализирую сессию NHibernate
      //Logic.NHibernateConfiguration.ConnectionString = @"Data Source=192.168.1.8\SQLEXPRESS;Initial Catalog=mlm_exchange;Integrated Security=False;Persist Security Info=True;User ID=sa;Password=masterkey;MultipleActiveResultSets=True";
      Logic.NHibernateConfiguration.ConnectionString = @"Data Source=IROTAEV-PC\SQLEXPRESS;Initial Catalog=mc_exchange;Integrated Security = SSPI;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";

      //TODO:Rtv переделать NHibernateConfiguration на static
      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().TryOpenSession(SessionStorageType.ThreadStatic);
      _NHibernaetSession = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;
      _NHibernaetSession.BeginTransaction();
      #endregion
    }

    protected readonly ISession _NHibernaetSession;

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

  /// <summary>
  /// Базовый тест настройки данных. 
  /// Используется, когда необходимо настроить данные приложения, 
  /// произвести какие-либо манипуляции и т.д.
  /// 
  /// Например, создание дополнительных пользователей, либо внесение платежей, 
  /// там где логика внесения платежей не может быть задействована
  /// 
  /// Использовать с осторожностью, т.к. данные тесты не повторяют логику приложения
  /// </summary>
  public abstract class AbstractPreformDataTest : AbstractTest
  {
  }
}
