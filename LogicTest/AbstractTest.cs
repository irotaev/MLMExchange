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

      #region Инициализирую сессию NHibernate
    
         Logic.NHibernateConfiguration.ConnectionString= @"Server=CTRLSOFT-FVJ87G;Database=mc_exchange;Integrated Security=True;";
      //Logic.NHibernateConfiguration.ConnectionString = @"Data Source=192.168.1.8\SQLEXPRESS;Initial Catalog=mlm_exchange;Integrated Security=False;Persist Security Info=True;User ID=sa;Password=masterkey;MultipleActiveResultSets=True";
      //Logic.NHibernateConfiguration.ConnectionString = @"Data Source=IROTAEV-PC\SQLEXPRESS;Initial Catalog=mc_exchange;Integrated Security = SSPI;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
      //Logic.NHibernateConfiguration.ConnectionString = @"Data Source=SUPER_MEN4iG-PC;Initial Catalog=mc_exchange;Integrated Security = SSPI;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
      //Logic.NHibernateConfiguration.ConnectionString = "Data Source=gladiolus.arvixe.com;Initial Catalog=mc_exchange8;Integrated Security=False;Persist Security Info=True;User ID=Rtv;Password=rtvrtvrtv;MultipleActiveResultSets=True";

      //TODO:Rtv переделать NHibernateConfiguration на static
      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().TryOpenSession(SessionStorageType.ThreadStatic);
      _NHibernaetSession = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;
      _NHibernaetSession.BeginTransaction();
      #endregion
    }

    public AbstractTest()
    {
    }

    public const string _DefaultUserPassword = "5a2d812ea05692ed5a25cc4b88d4dd14"; // Пароль: 12345678;
    protected static ISession _NHibernaetSession;

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

  #region Exceptions

  /// <summary>
  /// Абстрактная ошибка теста
  /// </summary>
  public abstract class AbstractTestException : Exception
  {
    public AbstractTestException() : base() { }
    public AbstractTestException(String message) : base(message) { }
    public AbstractTestException(String message, Exception ex) : base(message, ex) { }
  }

  /// <summary>
  /// Базовая ошибка теста
  /// </summary>
  public class TestException : AbstractTestException
  {
    public TestException() : base() { }
    public TestException(String message) : base(message) { }
    public TestException(String message, Exception ex) : base(message, ex) { }
  }

  /// <summary>
  /// Базовая ошибка теста. Параметр Null.
  /// </summary>
  public class TestParametrNullException : TestException
  {
    public TestParametrNullException(string parametr)
    {
      _Message = String.Format("Не задан параметр {0}", parametr);
    }

    private readonly string _Message;

    public override string Message
    {
      get
      {
        return _Message;
      }
    }
  }
  #endregion
}
