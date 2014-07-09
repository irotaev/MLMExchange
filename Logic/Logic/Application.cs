﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate.Linq;
using System.Threading;

namespace Logic
{
  /// <summary>
  /// Прокси-объект приложения.
  /// Настройка всего приложения в целом. Один объект на всю систему. Создается один раз при запуске биржи.
  /// Синглтон
  /// </summary>
  public sealed class Application : AbstractLogicObject<D_Application>
  {
    static Application()
    {
      #region Создие объекта приложения
      D_Application d_application = new D_Application();

      Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Save(d_application);
      #endregion
    }

    private Application(D_Application d_application) : base(d_application) { }

    private static bool IsInitialized;

    /// <summary>
    /// Получить экземпляр объекта. Реализация паттерна Синглтон
    /// </summary>
    public static Application Instance
    {
      get
      {
        D_Application d_application = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
          .Query<D_Application>().FirstOrDefault();

        return new Application(d_application);
      }
    }

    /// <summary>
    /// Получить текущие настройки для системы.
    /// Аналогично их можно получить из SystemSettings
    /// </summary>
    public SystemSettings CurrentSystemSettings
    {
      get
      {
        return SystemSettings.GetCurrentSestemSettings();
      }
    }

    /// <summary>
    /// Инициализировать приложение.
    /// Приложение можно инициализировать только один раз
    /// </summary>
    public void Initiliaze()
    {
      if (IsInitialized)
        return;

      #region Запустить процесс обеспечения доходности торговым сессиям
      StartTradingSessionsEnsureProfitabilityProcess();
      #endregion

      AddAdministratorRoleUsers();

      IsInitialized = true;
    }

    /// <summary>
    /// Добавить пользователей с ролью администратора в проект
    /// </summary>
    private void AddAdministratorRoleUsers()
    {
      #region Создаю администраторов
      D_User adminUser = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_User>().Where(x => x.Login == "administrator_irotaev").FirstOrDefault();

      if (adminUser == null)
      {
        adminUser = new D_User
        {
          Login = "administrator_irotaev",
          PasswordHash = "5a2d812ea05692ed5a25cc4b88d4dd14", // Пароль: 12345678
          PaymentSystemGroup = new D_PaymentSystemGroup()
        };

        adminUser.Roles = new List<D_AbstractRole> { new D_AdministratorRole { User = adminUser } };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(adminUser);
      }
      #endregion

      #region Создаю пользователя-систему
      D_System_User d_systemUser = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_System_User>().FirstOrDefault();

      if (d_systemUser == null)
      {
        D_PaymentSystemGroup paymentSystemGroup = new D_PaymentSystemGroup();
        paymentSystemGroup.BankPaymentSystems.Add(new D_BankPaymentSystem
              {
                BankName = "Fake",
                BIK = "123",
                CardNumber = "123",
                CurrentAccount = "123",
                INN = "123",
                IsDefault = true,
                KPP = "123",
                UserName = "System",
                UserPatronymic = "System",
                UserSurname = "System"
              });

        paymentSystemGroup.ElectronicPaymentSystems.Add(new D_ElectronicPaymentSystem
              {
                ElectronicName = "FakeElectronicPaymentSystem",
                PurseNumber = "123",
                IsDefault = true
              });

        d_systemUser = new D_System_User
        {
          Login = "system_user",
          PasswordHash = "5a2d812ea05692ed5a25cc4b88d4dd14", // Пароль: 12345678
          PaymentSystemGroup = paymentSystemGroup
        };

        d_systemUser.Roles = new List<D_AbstractRole> { new D_AdministratorRole { User = d_systemUser } };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(d_systemUser);
      }
      #endregion

#if DEBUG
      #region Создаю тестовых пользователей системы
      if (Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<D_User>().Where(x => x.Login == "irotaev" || x.Login == "newbik").Count() == 0)
      {
        D_User leaderUser = new D_User
        {
          Login = "leader",
          PasswordHash = "5a2d812ea05692ed5a25cc4b88d4dd14", // Пароль: 12345678
          PaymentSystemGroup = new D_PaymentSystemGroup
          {
            BankPaymentSystems = new List<D_BankPaymentSystem>
            {
              new D_BankPaymentSystem
              {
                BankName = "Test bank name",
                BIK = "123",
                CardNumber = "123",
                CurrentAccount = "123",
                INN = "123",
                IsDefault = true,
                KPP = "123",
                UserName = "leader",
                UserPatronymic = "Val",
                UserSurname = "Rtv"
              }
            },
            ElectronicPaymentSystems = new List<D_ElectronicPaymentSystem>
            {
              new D_ElectronicPaymentSystem
              {
                ElectronicName = "Test electronic for irotaev",
                IsDefault = false,
                PurseNumber = "123"
              }
            }
          },
          Name = "Андрей",
          Surname = "Ротаев",
          Patronymic = "Валерьевич",
          Email = "irotaev@gmail.com",
        };

        leaderUser.Roles = new List<D_AbstractRole> { new D_UserRole { User = leaderUser, MyCryptCount = 10000 }, new D_LeaderRole { User = leaderUser } };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(leaderUser);

        D_User testerUser = new D_User
        {
          Login = "tester",
          PasswordHash = "5a2d812ea05692ed5a25cc4b88d4dd14", // Пароль: 12345678
          PaymentSystemGroup = new D_PaymentSystemGroup
          {
            BankPaymentSystems = new List<D_BankPaymentSystem>
            {
              new D_BankPaymentSystem
              {
                BankName = "Test bank name",
                BIK = "123",
                CardNumber = "123",
                CurrentAccount = "123",
                INN = "123",
                IsDefault = true,
                KPP = "123",
                UserName = "tester",
                UserPatronymic = "Val",
                UserSurname = "Rtv"
              }
            },
            ElectronicPaymentSystems = new List<D_ElectronicPaymentSystem>
            {
              new D_ElectronicPaymentSystem
              {
                ElectronicName = "Test electronic for newbik",
                IsDefault = false,
                PurseNumber = "123"
              }
            }
          },
          Name = "Ньюбик",
          Surname = "Петрович",
          Patronymic = "Семенов",
          Email = "newbik@gmail.com"
        };

        testerUser.Roles = new List<D_AbstractRole> { new D_UserRole { User = testerUser, MyCryptCount = 5000 }, new D_TesterRole { User = testerUser } };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(testerUser);

        D_User brokerUser = new D_User
        {
          Login = "broker",
          PasswordHash = "5a2d812ea05692ed5a25cc4b88d4dd14", // Пароль: 12345678
          PaymentSystemGroup = new D_PaymentSystemGroup
          {
            BankPaymentSystems = new List<D_BankPaymentSystem>
            {
              new D_BankPaymentSystem
              {
                BankName = "Test bank name",
                BIK = "123",
                CardNumber = "123",
                CurrentAccount = "123",
                INN = "123",
                IsDefault = true,
                KPP = "123",
                UserName = "broker",
                UserPatronymic = "Val",
                UserSurname = "Rtv"
              }
            },
            ElectronicPaymentSystems = new List<D_ElectronicPaymentSystem>
            {
              new D_ElectronicPaymentSystem
              {
                ElectronicName = "Test electronic for newbik",
                IsDefault = false,
                PurseNumber = "123"
              }
            }
          },
          Name = "Брокерович",
          Surname = "Алексей",
          Patronymic = "Михайлович",
          Email = "broker@gmail.com"
        };

        brokerUser.Roles = new List<D_AbstractRole> { new D_UserRole { User = brokerUser, MyCryptCount = 7000 }, new D_BrokerRole { User = brokerUser } };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(brokerUser);
      }
      #endregion

      #region Создаю первую SystemSettings
      if (Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<D_SystemSettings>().Count() == 0)
      {
        D_SystemSettings firstSystemSettings = new D_SystemSettings
        {
          CheckPaymentPercent = 5,
          Quote = 10,
          ProfitPercent = 20,
          TradingSessionDuration = 2,
          MaxMyCryptCount = 10000
        };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(firstSystemSettings);
      }
      #endregion
#endif
    }

    /// <summary>
    /// Запустить процесс обеспечения доходности торговым сессиям.
    /// Процесс запускается в отдельном потоке.
    /// Поток пораждает бесконечное количество новых потоков обеспечения торговых сессий
    /// </summary>
    private void StartTradingSessionsEnsureProfitabilityProcess()
    {
      Thread tradingSessionEnsureThread = new Thread(() =>
        {
          while (true)
          {
            Thread thread = new Thread(() =>
            {
              Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().TryOpenSession(SessionStorageType.ThreadStatic);
              Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.BeginTransaction();

              new TradingSessionList().EnsureProfibilityOfTradingSessions();

              var nhibernateManager = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<Logic.INHibernateManager>();
              nhibernateManager.Session.Transaction.Commit();
              nhibernateManager.Session.Transaction.Dispose();
              nhibernateManager.Session.Dispose();
            });

            thread.Start();

            System.Threading.Thread.Sleep(5000);
          }
        });

      tradingSessionEnsureThread.Start();
    }
  }
}
