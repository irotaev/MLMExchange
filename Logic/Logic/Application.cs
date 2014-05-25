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
          PasswordHash = "25d55ad283aa400af464c76d713c07ad", // Пароль: 12345678,
          PaymentSystemGroup = new PaymentSystemGroup()
        };

        adminUser.Roles = new List<D_AbstractRole> { new D_AdministratorRole { User = adminUser } };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(adminUser);
      }
      #endregion

#if DEBUG
      #region Создаю тестовых пользователей системы
      if (Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<D_User>().Where(x => x.Login == "irotaev" || x.Login == "newbik").Count() == 0)
      {
        D_User irotaevUser = new D_User
        {
          Login = "irotaev",
          PasswordHash = "25d55ad283aa400af464c76d713c07ad", // Пароль: 12345678,
          PaymentSystemGroup = new PaymentSystemGroup(),
          Name = "Андрей",
          Surname = "Ротаев",
          Patronymic = "Валерьевич",
          Email = "irotaev@gmail.com"
        };

        irotaevUser.Roles = new List<D_AbstractRole> { new D_UserRole { User = irotaevUser } };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(irotaevUser);

        D_User newbikUser = new D_User
        {
          Login = "newbik",
          PasswordHash = "25d55ad283aa400af464c76d713c07ad", // Пароль: 12345678,
          PaymentSystemGroup = new PaymentSystemGroup(),
          Name = "Ньюбик",
          Surname = "Петрович",
          Patronymic = "Семенов",
          Email = "newbik@gmail.com"
        };

        newbikUser.Roles = new List<D_AbstractRole> { new D_UserRole { User = newbikUser } };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(newbikUser);
      }
      #endregion

      #region Создаю первую SystemSettings
      if (Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<D_SystemSettings>().Count() == 0)
      {
        D_SystemSettings firstSystemSettings = new D_SystemSettings
        {
          CheckPaymentPercent = 5,
          Quote = 10
        };

        Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Save(firstSystemSettings);
      }
      #endregion
#endif
    }

    /// <summary>
    /// Запустить процесс обеспечения доходности торговым сессиям.
    /// Процесс запускается в отдельном БЕСКОНЕЧНОМ потоке
    /// </summary>
    private void StartTradingSessionsEnsureProfitabilityProcess()
    {
      while(false)
      {        
        Thread thread = new Thread(() => 
        {
          new TradingSessionList().EnsureProfibilityOfTradingSessions();
        });

        thread.Start();

        System.Threading.Thread.Sleep(2000);
      }
    }
  }
}