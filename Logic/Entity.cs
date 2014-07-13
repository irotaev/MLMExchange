﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NHibernate.Event;
using NHibernate.Linq;
using Logic.Lib;

namespace Logic
{
  #region Entity
  public abstract class D_BaseObject : IEntityObject
  {
    public virtual long Id { get; set; }
    public virtual DateTime CreationDateTime { get; set; }
  }

  #region System
  /// <summary>
  /// Объект системы
  /// </summary>
  public class D_Application : D_BaseObject
  {
    internal D_Application() { }

    /// <summary>
    /// Денежный резерв.
    /// Сколько денег перечислили в резерв фонда
    /// </summary>
    public virtual decimal MoneyReserv { get; set; }
  }

  /// <summary>
  /// Настройки системы
  /// </summary>
  public class D_SystemSettings : D_BaseObject
  {
    /// <summary>
    /// Процент проверочного платежа
    /// </summary>
    public virtual decimal CheckPaymentPercent { get; set; }
    /// <summary>
    /// Котировка
    /// </summary>
    public virtual int Quote { get; set; }
    /// <summary>
    /// Длительность торговой сессии.
    /// Измеряется в минутах
    /// </summary>
    public virtual decimal TradingSessionDuration { get; set; }
    /// <summary>
    /// Процент доходности для продавца. Измеряется в процентах
    /// </summary>
    public virtual decimal ProfitPercent { get; set; }
    /// <summary>
    /// Максимальное колличество mycrypto при заказе
    /// </summary>
    public virtual long MaxMyCryptCount { get; set; }
  }
  #endregion

  #region Пользователи
  public class D_User : D_BaseObject
  {
    public D_User()
    {
      Roles = new List<D_AbstractRole>();
      //UserRole = new List<D_UserRole>();
    }

    public virtual string Login { get; set; }
    public virtual string PasswordHash { get; set; }
    public virtual string Name { get; set; }
    public virtual string Surname { get; set; }
    public virtual string Patronymic { get; set; }
    public virtual string Email { get; set; }
    /// <summary>
    /// Относительный путь к фото пользователя
    /// </summary>
    public virtual string PhotoRelativePath { get; set; }
    //public virtual IList<Payment> Payments { get; set; }
    /// <summary>
    /// Группа платежных систем
    /// </summary>
    public virtual D_PaymentSystemGroup PaymentSystemGroup { get; set; }
    /// <summary>
    /// Роли пользователя в системе
    /// </summary>
    public virtual IList<D_AbstractRole> Roles { get; set; }
    /// <summary>
    /// Тип пользователя
    /// </summary>
    public virtual D_UserType UserType { get; set; }
    //public virtual IList<D_UserRole> UserRole { get; set; }
    /// <summary>
    /// Роль пользователя, являющаяся босом для данного реферала
    /// </summary>
    public virtual D_UserRole ReferalBossRole { get; set; }
  }

  /// <summary>
  /// Система-пользователь. 
  /// Пользователь, являющийся системой
  /// </summary>
  public class D_System_User : D_User
  {
  }

  public enum D_UserType : int
  {
    /// <summary>
    /// Базовый пользователь системы
    /// </summary>
    BaseUser = 0,
    /// <summary>
    /// Система-пользователь. Пользователь-робот системы
    /// </summary>
    SystemUser = 1
  }
  #endregion

  #region Roles
  /// <summary>
  /// Базовая роль системы
  /// </summary>
  public abstract class D_AbstractRole : D_BaseObject
  {
    /// <summary>
    /// Пользователь, у которого имеется данная роль
    /// </summary>
    public virtual D_User User { get; set; }
    /// <summary>
    /// Тип роли. Используется для наследования
    /// </summary>
    public virtual RoleType RoleType { get; set; }
  }

  /// <summary>
  /// Тип роли
  /// </summary>
  public enum RoleType : int
  {
    /// <summary>
    /// Пользователь системы. Администратор.
    /// </summary>
    Administrator = 1,
    /// <summary>
    /// Пользователь системы. Участник.
    /// </summary>
    User = 0,
    /// <summary>
    /// Пользователь системы. Лидер.
    /// </summary>
    Leader = 2,
    /// <summary>
    /// Пользователь системы. Тестер.
    /// </summary>
    Tester = 3,
    /// <summary>
    /// Пользователь системы. Брокер.
    /// </summary>
    Broker = 4
  }

  /// <summary>
  /// Роль пользователя системы. Участник.
  /// </summary>
  public class D_UserRole : D_AbstractRole
  {
    /// <summary>
    /// Количество my-crypt.
    /// </summary>
    public virtual long MyCryptCount { get; set; }
    /// <summary>
    /// Список рефералов
    /// </summary>
    public virtual IList<D_User> ReferalUsers { get; set; }
  }

  /// <summary>
  /// Роль участника системы. Лидер.
  /// </summary>
  public class D_LeaderRole : D_AbstractRole
  {
  }

  /// <summary>
  /// Роль участника системы. Тестер.
  /// </summary>
  public class D_TesterRole : D_AbstractRole
  {
  }

  /// <summary>
  /// Роль участника системы. Брокер.
  /// </summary>
  public class D_BrokerRole : D_AbstractRole
  {
  }

  /// <summary>
  /// Роль администратора системы
  /// </summary>
  public class D_AdministratorRole : D_AbstractRole
  {
    public D_AdministratorRole()
    {
      RoleType = Logic.RoleType.Administrator;
    }
  }
  #endregion

  #region Платежи

  #region Платеж
  public class Payment : D_BaseObject
  {
    /// <summary>
    /// Пользователь, который совершил платеж
    /// </summary>
    public virtual D_User Payer { get; set; }
    /// <summary>
    /// Количество реальных денег по платежу
    /// </summary>
    public virtual decimal RealMoneyAmount { get; set; }
    /// <summary>
    /// Платежная система, по которой осу-ществлялся платеж
    /// </summary>
    public virtual D_PaymentSystem PaymentSystem { get; set; }
    /// <summary>
    /// Счет, к которому превязан платеж
    /// </summary>
    public virtual D_Bill Bill { get; set; }
  }
  #endregion

  #region Счет
  public class D_Bill : D_BaseObject
  {
    private IList<Payment> _Payments = new List<Payment>();

    /// <summary>
    /// Пользователь, которому оплачивают счет.
    /// Тот, кто принемает платеж. Необязательный параметр
    /// </summary>
    public virtual D_User PaymentAcceptor { get; set; }
    /// <summary>
    /// Кому выставлен счет. 
    /// Пользователь, который должен оплатить счет
    /// </summary>
    public virtual D_User Payer { get; set; }
    /// <summary>
    /// Количество денег по счету
    /// </summary>
    public virtual decimal MoneyAmount { get; set; }
    /// <summary>
    /// Платежы по счету.
    /// Добавление платежей идет строго через прокси-объект
    /// </summary>
    public virtual IEnumerable<Payment> Payments { get { return _Payments.ToList(); } set { _Payments = value.ToList(); } }
    /// <summary>
    /// Состояние оплаты
    /// </summary>
    public virtual BillPaymentState PaymentState { get; set; }

    /// <summary>
    /// Необходимо ли довнести деньги
    /// </summary>
    public virtual bool IsNeedSubstantialMoney
    {
      get
      {
        decimal totalPaymetsMoney = 0;

        Payments.ForEach(p =>
        {
          totalPaymetsMoney += p.RealMoneyAmount;
        });

        return totalPaymetsMoney < MoneyAmount;
      }
    }
    /// <summary>
    /// Тип счета
    /// </summary>
    public virtual BillType BillType { get; set; }

    /// <summary>
    /// Получить список платежей
    /// </summary>
    /// <returns></returns>
    public virtual IList<Payment> GetPaymentList()
    {
      return _Payments;
    }
  }

  /// <summary>
  /// Счет на оплату торговых обязательств
  /// </summary>
  [DataConfig(LogicProxyType = typeof(YieldSessionBill))]
  public class D_YieldSessionBill : D_Bill
  {
    public D_YieldSessionBill()
    {
      BillType = Logic.BillType.YieldSessionBill;
    }

    /// <summary>
    /// Торговая сессия того, кто оплачивает счет
    /// </summary>
    public virtual D_TradingSession PayerTradingSession { get; set; }

    /// <summary>
    /// Торговая сессия того, кто принемает платеж
    /// </summary>
    public virtual D_TradingSession AcceptorTradingSession { get; set; }
  }

  /// <summary>
  /// Состояние счета
  /// </summary>
  public enum BillPaymentState : int
  {
    NA = 0,    
    /// <summary>
    /// Ожидание платежа
    /// </summary>
    WaitingPayment = 4,
    /// <summary>
    /// Оплачен
    /// </summary>
    Paid = 1,
    /// <summary>
    /// Внесено достаточно денег по счету
    /// </summary>
    EnoughMoney = 2,
    /// <summary>
    /// Не оплачен
    /// </summary>
    NotPaid = 3
  }

  /// <summary>
  /// Тип счета
  /// </summary>
  public enum BillType : int
  {
    BaseBill = 0,
    YieldSessionBill = 1
  }
  #endregion

  #endregion

  #region Платежные системы
  public class D_PaymentSystem : D_BaseObject
  {
    /// <summary>
    /// Дефолтная система - true, нет - false
    /// </summary>
    public virtual bool IsDefault { get; set; }
    /// <summary>
    /// Группа платежных систем
    /// </summary>
    public virtual D_PaymentSystemGroup PaymentSystemGroup { get; set; }
  }

  /// <summary>
  /// Банковская платежная система
  /// </summary>
  public class D_BankPaymentSystem : D_PaymentSystem
  {
    public virtual string UserName { get; set; }
    public virtual string UserSurname { get; set; }
    public virtual string UserPatronymic { get; set; }
    /// <summary>
    /// Наименование банка
    /// </summary>
    public virtual string BankName { get; set; }
    /// <summary>
    /// Инн
    /// </summary>
    public virtual string INN { get; set; }
    /// <summary>
    /// КПП
    /// </summary>
    public virtual string KPP { get; set; }
    /// <summary>
    /// Расчетный счет
    /// </summary>
    public virtual string CurrentAccount { get; set; }
    /// <summary>
    /// БИК банка
    /// </summary>
    public virtual string BIK { get; set; }
    /// <summary>
    /// Номер карты
    /// </summary>
    public virtual string CardNumber { get; set; }
  }

  /// <summary>
  /// Электронная платёжная система
  /// </summary>
  public class D_ElectronicPaymentSystem : D_PaymentSystem
  {
    /// <summary>
    /// Название электронной платёжной системы
    /// </summary>
    public virtual string ElectronicName { get; set; }
    /// <summary>
    /// Номер кошелька
    /// </summary>
    public virtual string PurseNumber { get; set; }
  }

  /// <summary>
  /// Группа платежных систем
  /// </summary>
  public class D_PaymentSystemGroup : D_BaseObject
  {
    public D_PaymentSystemGroup()
    {
      BankPaymentSystems = new List<D_BankPaymentSystem>();
      ElectronicPaymentSystems = new List<D_ElectronicPaymentSystem>();
    }

    /// <summary>
    /// Платежные системы типа банк
    /// </summary>
    public virtual IList<D_BankPaymentSystem> BankPaymentSystems { get; set; }
    /// <summary>
    /// Платёжные системы типа электронные
    /// </summary>
    public virtual IList<D_ElectronicPaymentSystem> ElectronicPaymentSystems { get; set; }
    /// <summary>
    /// Кому пренадлежит группа платежных систем
    /// </summary>
    public virtual D_User User { get; set; }
  }
  #endregion

  #region D_AddMyCryptTransaction
  /// <summary>
  /// Запрос на добавление my-crypt
  /// </summary>
  public class D_AddMyCryptTransaction : D_BaseObject
  {
    /// <summary>
    /// Пополняемое количество MyCrypt
    /// </summary>
    public virtual long MyCryptCount { get; set; }
    public virtual string Comment { get; set; }
    /// <summary>
    /// Кто посылает запрос на получение my-crypt
    /// </summary>
    public virtual D_User User { get; set; }
    /// <summary>
    /// Относительный путь к подтверждающей картинке
    /// </summary>
    public virtual string ImageRelativePath { get; set; }
    /// <summary>
    /// Состояние запроса
    /// </summary>
    public virtual AddMyCryptTransactionState State { get; set; }
  }

  /// <summary>
  /// Статус добавление my-crypt
  /// </summary>
  public enum AddMyCryptTransactionState : int
  {
    /// <summary>
    /// Статус еще не применен
    /// </summary>
    NA = 0,
    /// <summary>
    /// Утвержден
    /// </summary>
    Approved = 1,
    /// <summary>
    /// Не утвержден
    /// </summary>
    NotApproved = 2
  }
  #endregion

  #region BiddingParticipateApplication
  /// <summary>
  /// Предложения продажи my-crypt
  /// </summary>
  public class BiddingParticipateApplication : D_BaseObject
  {
    /// <summary>
    /// Продавец
    /// </summary>
    public virtual D_User Seller { get; set; }
    /// <summary>
    /// Количество my-crypt на продажу
    /// </summary>
    public virtual long MyCryptCount { get; set; }
    /// <summary>
    /// Запросы на приобретение my-crypt по данной заявке
    /// </summary>
    public virtual IList<BuyingMyCryptRequest> BuyingMyCryptRequests { get; set; }
    /// <summary>
    /// Состояние заявки
    /// </summary>
    public virtual BiddingParticipateApplicationState State { get; set; }
    /// <summary>
    /// Торговая сессия
    /// </summary>
    public virtual D_TradingSession TradingSession { get; set; }
  }

  /// <summary>
  /// Состояние заявки на участие в торгах
  /// </summary>
  public enum BiddingParticipateApplicationState : int
  {
    NA = 0,
    /// <summary>
    /// Подана
    /// </summary>
    Filed = 1,
    /// <summary>
    /// Отменена заявка
    /// </summary>
    Cancelled = 2,
    /// <summary>
    /// Принят запрос от покупателя
    /// </summary>
    Accepted = 3,
    /// <summary>
    /// Закрыта
    /// </summary>
    Closed = 4
  }
  #endregion

  #region BuyingMyCryptRequest
  /// <summary>
  /// Запрос на покупку my-crypt
  /// </summary>
  public class BuyingMyCryptRequest : D_BaseObject
  {
    /// <summary>
    /// Пользователь, к которому привязан запрос на покупку
    /// </summary>
    public virtual D_User SellerUser { get; set; }
    /// <summary>
    /// Покупатель
    /// </summary>
    public virtual D_User Buyer { get; set; }
    /// <summary>
    /// Количество myc-crypt на покупку
    /// </summary>
    public virtual long MyCryptCount { get; set; }
    /// <summary>
    /// Комментарии
    /// </summary>
    public virtual string Comment { get; set; }
    /// <summary>
    /// Предложения продажи my-crypt, к которой прикреплена данная заявка
    /// </summary>
    public virtual BiddingParticipateApplication BiddingParticipateApplication { get; set; }
    /// <summary>
    /// Состояние запроса
    /// </summary>
    public virtual BuyingMyCryptRequestState State { get; set; }
    /// <summary>
    /// Торговая сессия
    /// </summary>
    public virtual D_TradingSession TradingSession { get; set; }
  }

  public enum BuyingMyCryptRequestState : int
  {
    /// <summary>
    /// Не указано
    /// </summary>
    NA = 0,
    /// <summary>
    /// Ждет подтверждения
    /// </summary>
    AwaitingConfirm = 1,
    /// <summary>
    /// Отклонен
    /// </summary>
    Denied = 2,
    /// <summary>
    /// Принят
    /// </summary>
    Accepted = 3
  }

  public static class BuyingMyCryptRequestStateExtension
  {
    public static string ToLocal(this BuyingMyCryptRequestState state)
    {
      switch (state)
      {
        case BuyingMyCryptRequestState.Denied:
          return Logic.Properties.GeneralResources.Denied;
        case BuyingMyCryptRequestState.AwaitingConfirm:
          return Logic.Properties.GeneralResources.AwaitingConfirm;
        case BuyingMyCryptRequestState.Accepted:
          return Logic.Properties.GeneralResources.Accepted;
        case BuyingMyCryptRequestState.NA:
        default:
          return Logic.Properties.GeneralResources.NA;
      }
    }
  }
  #endregion

  #region TradingSession
  /// <summary>
  /// Торговая сессия
  /// </summary>
  /// 
  public class D_TradingSession : D_BaseObject
  {
    /// <summary>
    /// Заявка на участие в торгах
    /// </summary>
    public virtual BiddingParticipateApplication BiddingParticipateApplication { get; set; }
    /// <summary>
    /// Запрос на покупку my-crypt
    /// </summary>
    public virtual BuyingMyCryptRequest BuyingMyCryptRequest { get; set; }
    /// <summary>
    /// Счет на прогверочный платеж
    /// </summary>
    public virtual D_Bill CheckBill { get; set; }
    /// <summary>
    /// Счет на комисионный сбор продавца
    /// </summary>
    public virtual D_Bill SallerInterestRateBill { get; set; }
    /// <summary>
    /// Статус торговой сессии
    /// </summary>
    public virtual TradingSessionStatus State { get; set; }
    /// <summary>
    /// Счета на оплату доходности торговой сессии
    /// </summary>
    public virtual IList<D_YieldSessionBill> YieldSessionBills { get; set; }
    /// <summary>
    /// Дата и время когда можно будет закрыть сессию
    /// </summary>
    public virtual DateTime? ClosingSessionDateTime { get; set; }
    /// <summary>
    /// Системные настройки, привязанные к данной торговой сессии
    /// </summary>
    public virtual D_SystemSettings SystemSettings { get; set; }
    /// <summary>
    /// Дата последнего добавления счета роботом-поисковиком, который ищет пользователей 
    /// для удовлетворения доходности текущей торговой сессии.
    /// </summary>
    public virtual DateTime? DateLastYieldTradingSessionUnsureSearchRobotAddBill { get; set; }
  }

  public enum TradingSessionStatus : int
  {
    NA = 0,
    /// <summary>
    /// Открыта
    /// </summary>
    Open = 1,
    /// <summary>
    /// Необходимо обеспечить доходность
    /// </summary>
    NeedEnsureProfibility = 5,
    /// <summary>
    /// Ожидание начатия торговой сессии 
    /// </summary>
    WaitForProgressStart = 7,
    /// <summary>
    /// Сессия исполняется. 
    /// Ожидание периода между оплатой доходности сессии и получения прибыли
    /// </summary>
    SessionInProgress = 2,
    /// <summary>
    /// Торговая сессия ждет прибыли
    /// </summary>
    NeedProfit = 6,
    /// <summary>
    /// Подтверждение прибыли пользователем
    /// </summary>
    ProfitConfirmation = 8,
    /// <summary>
    /// Закрыта
    /// </summary>
    Closed = 3,
    /// <summary>
    /// Забанена. Допущены нарушения в ходе торговой сесcии
    /// </summary>
    Baned = 4
  }
  #endregion
  #endregion

  #region Map Entity
  public abstract class D_BaseObject_Map<TObject> : ClassMap<TObject> where TObject : D_BaseObject
  {
    public D_BaseObject_Map()
    {
      Id(x => x.Id).GeneratedBy.HiLo("10").CustomType<Int64>();

      Map(x => x.CreationDateTime).Not.Nullable();
    }
  }

  public class D_Application_Map : D_BaseObject_Map<D_Application>
  {
    public D_Application_Map()
    {
      Map(x => x.MoneyReserv);
    }
  }

  public class D_SystemSettings_Map : D_BaseObject_Map<D_SystemSettings>
  {
    public D_SystemSettings_Map()
    {
      Map(x => x.CheckPaymentPercent).Not.Nullable();
      Map(x => x.Quote).Not.Nullable();
      Map(x => x.ProfitPercent).Not.Nullable();
      Map(x => x.TradingSessionDuration).Not.Nullable();
      Map(x => x.MaxMyCryptCount).Not.Nullable();
    }
  }

  public class D_User_Map : D_BaseObject_Map<D_User>
  {
    public D_User_Map()
    {
      Map(x => x.Login).Not.Nullable().Length(100);
      Map(x => x.PasswordHash).Not.Nullable().Length(200);
      Map(x => x.Name).Nullable().Length(100);
      Map(x => x.Surname).Nullable().Length(100);
      Map(x => x.Patronymic).Nullable().Length(100);
      Map(x => x.Email).Nullable().Length(100);
      Map(x => x.PhotoRelativePath).Nullable().Length(200);
      References(x => x.PaymentSystemGroup).Column("PaymentSystemGroupId").Unique().Cascade.All();
      HasMany(x => x.Roles).KeyColumn("UserId").Cascade.All();
      References(x => x.ReferalBossRole).Column("ReferalBossRoleId").Cascade.All();

      DiscriminateSubClassesOnColumn<D_UserType>("UserType", D_UserType.BaseUser);
    }
  }

  public class D_System_User_Map : SubclassMap<D_System_User>
  {
    public D_System_User_Map()
    {
      DiscriminatorValue(D_UserType.SystemUser);
    }
  }

  #region Roles
  public class D_AbstractRole_Map : D_BaseObject_Map<D_AbstractRole>
  {
    public D_AbstractRole_Map()
    {
      References(x => x.User).Column("UserId").Cascade.All();

      DiscriminateSubClassesOnColumn<RoleType>("RoleType", RoleType.User);
    }
  }

  public class D_UserRole_Map : SubclassMap<D_UserRole>
  {
    public D_UserRole_Map()
    {
      DiscriminatorValue(RoleType.User);

      Map(x => x.MyCryptCount).Default("0").Not.Nullable();
      HasMany(x => x.ReferalUsers).KeyColumn("ReferalBossRoleId").Cascade.SaveUpdate();
    }
  }

  public class D_LeaderRole_Map : SubclassMap<D_LeaderRole>
  {
    public D_LeaderRole_Map()
    {
      DiscriminatorValue(RoleType.Leader);
    }
  }

  public class D_TesterRole_Map : SubclassMap<D_TesterRole>
  {
    public D_TesterRole_Map()
    {
      DiscriminatorValue(RoleType.Tester);
    }
  }

  public class D_BrokerRole_Map : SubclassMap<D_BrokerRole>
  {
    public D_BrokerRole_Map()
    {
      DiscriminatorValue(RoleType.Broker);
    }
  }

  public class D_AdministratorRole_Map : SubclassMap<D_AdministratorRole>
  {
    public D_AdministratorRole_Map()
    {
      DiscriminatorValue(RoleType.Administrator);
    }
  }
  #endregion

  #region Платежи

  #region Платеж
  public class Payment_Map : D_BaseObject_Map<Payment>
  {
    public Payment_Map()
    {
      References(x => x.Payer).Column("UserId").Not.Nullable();
      Map(x => x.RealMoneyAmount).Not.Nullable();
      References(x => x.PaymentSystem).Column("PaymentSystemId").Nullable();
      References(x => x.Bill).Column("BillId").Nullable().Cascade.SaveUpdate();
    }
  }
  #endregion

  #region Счет
  public class D_Bill_Map : D_BaseObject_Map<D_Bill>
  {
    public D_Bill_Map()
    {
      References(x => x.PaymentAcceptor).Column("PaymentAcceptorId");
      References(x => x.Payer).Column("PayerId");
      Map(x => x.MoneyAmount).Nullable();
      Map(x => x.PaymentState).CustomType<BillPaymentState>();
      HasMany<Payment>(x => x.Payments).KeyColumn("BillId").Cascade.SaveUpdate();

      DiscriminateSubClassesOnColumn<BillType>("BillType", BillType.BaseBill);
    }
  }

  public class D_YieldSessionBill_Map : SubclassMap<D_YieldSessionBill>
  {
    public D_YieldSessionBill_Map()
    {
      DiscriminatorValue(BillType.YieldSessionBill);

      References(x => x.PayerTradingSession).Column("PayerTradingSessionId").Nullable().Cascade.SaveUpdate();
      References(x => x.AcceptorTradingSession).Column("AcceptorTradingSessionId").Nullable().Cascade.SaveUpdate();
    }
  }
  #endregion

  #endregion

  #region Платежные системы
  public class D_PaymentSystem_Map : D_BaseObject_Map<D_PaymentSystem>
  {
    public D_PaymentSystem_Map()
    {
      References<D_PaymentSystemGroup>(x => x.PaymentSystemGroup).Column("PaymentSystemGroupId").Cascade.SaveUpdate();
      Map(x => x.IsDefault);

      UseUnionSubclassForInheritanceMapping();
    }
  }

  public class BankPaymentSystem_Map : SubclassMap<D_BankPaymentSystem>
  {
    public BankPaymentSystem_Map()
    {
      Map(x => x.UserName).Not.Nullable();
      Map(x => x.UserSurname).Not.Nullable();
      Map(x => x.UserPatronymic).Not.Nullable();
      Map(x => x.BankName).Not.Nullable();
      Map(x => x.INN).Not.Nullable();
      Map(x => x.KPP).Not.Nullable();
      Map(x => x.CurrentAccount).Not.Nullable();
      Map(x => x.BIK).Not.Nullable();
      Map(x => x.CardNumber).Not.Nullable();
    }
  }

  public class ElectronicPaymentSystem_Map : SubclassMap<D_ElectronicPaymentSystem>
  {
    public ElectronicPaymentSystem_Map()
    {
      Map(x => x.ElectronicName).Not.Nullable();
      Map(x => x.PurseNumber).Not.Nullable();
    }
  }

  public class PaymentSystemGroup_Map : D_BaseObject_Map<D_PaymentSystemGroup>
  {
    public PaymentSystemGroup_Map()
    {
      HasMany<D_BankPaymentSystem>(x => x.BankPaymentSystems).KeyColumn("PaymentSystemGroupId").Cascade.All();
      HasMany<D_ElectronicPaymentSystem>(x => x.ElectronicPaymentSystems).KeyColumn("PaymentSystemGroupId").Cascade.All();
      HasOne(x => x.User).PropertyRef(x => x.PaymentSystemGroup).Cascade.SaveUpdate();
    }
  }
  #endregion

  public class D_AddMyCryptTransaction_Map : D_BaseObject_Map<D_AddMyCryptTransaction>
  {
    public D_AddMyCryptTransaction_Map()
    {
      Map(x => x.MyCryptCount).Not.Nullable();
      Map(x => x.Comment).Nullable().Length(5000);
      References(x => x.User).Column("UserId");
      Map(x => x.ImageRelativePath).Nullable().Length(1000);
      Map(x => x.State).CustomType<AddMyCryptTransactionState>();
    }
  }

  public class BiddingParticipateApplication_Map : D_BaseObject_Map<BiddingParticipateApplication>
  {
    public BiddingParticipateApplication_Map()
    {
      References(x => x.Seller).Not.Nullable().Column("SaylerId");
      Map(x => x.MyCryptCount).Not.Nullable();
      Map(x => x.State).CustomType<BiddingParticipateApplicationState>();
      HasMany<BuyingMyCryptRequest>(x => x.BuyingMyCryptRequests).KeyColumn("BiddingParticipateApplicationId").Inverse().Cascade.All();
      HasOne(x => x.TradingSession).PropertyRef(x => x.BiddingParticipateApplication).Cascade.SaveUpdate();
    }
  }

  public class BuyingMyCryptRequest_Map : D_BaseObject_Map<BuyingMyCryptRequest>
  {
    public BuyingMyCryptRequest_Map()
    {
      References(x => x.SellerUser).Not.Nullable().Column("SellerUserId");
      References(x => x.Buyer).Not.Nullable().Column("BuyerId");
      References(x => x.BiddingParticipateApplication, "BiddingParticipateApplicationId").Cascade.SaveUpdate();
      Map(x => x.MyCryptCount).Not.Nullable();
      Map(x => x.Comment).Nullable().Length(3000);
      Map(x => x.State).CustomType<BuyingMyCryptRequestState>();
      HasOne(x => x.TradingSession).PropertyRef(x => x.BuyingMyCryptRequest).Cascade.SaveUpdate();
    }
  }

  public class D_TradingSession_Map : D_BaseObject_Map<D_TradingSession>
  {
    public D_TradingSession_Map()
    {
      References(x => x.BiddingParticipateApplication).Column("BiddingParticipateApplicationId").Not.Nullable().Unique().Cascade.All();
      References(x => x.BuyingMyCryptRequest).Column("BuyingMyCryptRequestId").Not.Nullable().Unique().Cascade.All();
      References(x => x.CheckBill).Column("CheckBillId").Not.Nullable().Cascade.All();
      References(x => x.SallerInterestRateBill).Column("SallerInterestRateBillId").Not.Nullable().Cascade.All();
      References(x => x.SystemSettings).Not.Nullable();
      Map(x => x.State).CustomType<TradingSessionStatus>();
      HasMany<D_YieldSessionBill>(x => x.YieldSessionBills).KeyColumn("TradingSessionId").Cascade.All();
      Map(x => x.DateLastYieldTradingSessionUnsureSearchRobotAddBill).Nullable();
      Map(x => x.ClosingSessionDateTime).Nullable();
    }
  }
  #endregion

  #region Event listeners
  public class PreInsertEvent : IPreInsertEventListener
  {
    public bool OnPreInsert(NHibernate.Event.PreInsertEvent @event)
    {
      D_BaseObject baseObject = (@event.Entity as D_BaseObject);

      DataConfigAttribute dataConfigAttribute = baseObject.GetType().GetCustomAttributes(typeof(DataConfigAttribute), true).Cast<DataConfigAttribute>().FirstOrDefault();

      if (baseObject == null)
        return false;

      #region Задаю время создания
      int createdDateTimeIndex = Array.IndexOf(@event.Persister.PropertyNames, "CreationDateTime");

      DateTime creationDate = DateTime.UtcNow;
      @event.State[createdDateTimeIndex] = creationDate;
      baseObject.CreationDateTime = creationDate;
      #endregion

      if (dataConfigAttribute != null)
      {
        if (dataConfigAttribute.LogicProxyType != null)
        {
          INhibernateEvent nhibernateEvent = Activator.CreateInstance(dataConfigAttribute.LogicProxyType, new object[] { baseObject }) as INhibernateEvent;
          nhibernateEvent.OnPreInsert(@event);
        }
      }

      return false;
    }
  }

  public class PreUpdateEvent : IPreUpdateEventListener
  {
    public bool OnPreUpdate(NHibernate.Event.PreUpdateEvent @event)
    {
      D_BaseObject baseObject = (@event.Entity as D_BaseObject);

      DataConfigAttribute dataConfigAttribute = baseObject.GetType().GetCustomAttributes(typeof(DataConfigAttribute), true).Cast<DataConfigAttribute>().FirstOrDefault();

      if (baseObject == null)
        return false;

      if (dataConfigAttribute != null)
      {
        if (dataConfigAttribute.LogicProxyType != null)
        {
          INhibernateEvent nhibernateEvent = Activator.CreateInstance(dataConfigAttribute.LogicProxyType, new object[] { baseObject }) as INhibernateEvent;
          nhibernateEvent.OnPreUpdate(@event);
        }
      }

      return false;
    }
  }
  #endregion
}
