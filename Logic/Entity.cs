using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using NHibernate.Event;

namespace Logic
{
  #region Entity
  public abstract class BaseObject
  {
    public virtual long Id { get; set; }
    public virtual DateTime CreationDateTime { get; set; }
  }

  public class User : BaseObject
  {
    public virtual string Login { get; set; }
    public virtual string PasswordHash { get; set; }
    public virtual string Name { get; set; }
    public virtual string Surname { get; set; }
    public virtual string Patronymic { get; set; }
    public virtual string Email { get; set; }
    public virtual string PhotoRelativePath { get; set; }
    public virtual IList<Payment> Payments { get; set; }
  }

  #region Платеж
  public class Payment : BaseObject
  {
    /// <summary>
    /// Пользователь, который совершил платеж
    /// </summary>
    public virtual User User { get; set; }
    /// <summary>
    /// Количество реальных денег по платежу
    /// </summary>
    public virtual decimal? RealMoneyAmount { get; set; }
  }

  /// <summary>
  /// Проверочный платеж системе
  /// </summary>
  public class BuyingMyCryptCheckPayment : Payment
  {
    /// <summary>
    /// Заявка на покупку my-crypt, к которой привязан платеж
    /// </summary>
    public virtual BuyingMyCryptRequest BuyingMyCryptRequest { get; set; }
    /// <summary>
    /// Состояние платежа
    /// </summary>
    public virtual BuyingMyCryptCheckPaymentState State { get; set; }
  }

  /// <summary>
  /// Состояние платежа
  /// </summary>
  public enum BuyingMyCryptCheckPaymentState : int
  {
    NA = 0,
    /// <summary>
    /// Подтвержден
    /// </summary>
    Approved = 1,
    /// <summary>
    /// Не подтвержден
    /// </summary>
    NotApproved
  }
  #endregion

  public class AddMyCryptTransaction : BaseObject
  {
    /// <summary>
    /// Пополняемое количество MyCrypt
    /// </summary>
    public virtual long MyCryptCount { get; set; }
    public virtual string Comment { get; set; }
    public virtual User User { get; set; }
    /// <summary>
    /// Относительный путь к подтверждающей картинке
    /// </summary>
    public virtual string ImageRelativePath { get; set; }
  }

  #region Заявка на участие в торгах
  /// <summary>
  /// Предложения продажи my-crypt
  /// </summary>
  public class BiddingParticipateApplication : BaseObject
  {
    /// <summary>
    /// Продавец
    /// </summary>
    public virtual User Seller { get; set; }
    /// <summary>
    /// Количество my-crypt на продажу
    /// </summary>
    public virtual long MyCryptCount { get; set; }
    /// <summary>
    /// Покупатель
    /// </summary>
    public virtual User Buyer { get; set; }
    /// <summary>
    /// Запросы на приобретение my-crypt по данной заявке
    /// </summary>
    public virtual IList<BuyingMyCryptRequest> BuyingMyCryptRequests { get; set; }
    /// <summary>
    /// Состояние заявки
    /// </summary>
    public virtual BiddingParticipateApplicationState State { get; set; }
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
    Accepted = 3
  }
  #endregion

  #region BuyingMyCryptRequest
  /// <summary>
  /// Запрос на покупку my-crypt
  /// </summary>
  public class BuyingMyCryptRequest : BaseObject
  {
    /// <summary>
    /// Пользователь, к которому привязан запрос на покупку
    /// </summary>
    public virtual User SellerUser { get; set; }
    /// <summary>
    /// Покупатель
    /// </summary>
    public virtual User Buyer { get; set; }
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
    /// Проверочный платеж по данной заявке
    /// </summary>
    public virtual BuyingMyCryptCheckPayment CheckPayment { get; set; }
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
      switch(state)
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
  #endregion

  #region Map Entity
  abstract public class BaseObject_Map<TObject> : ClassMap<TObject> where TObject : BaseObject
  {
    public BaseObject_Map()
    {
      Id(x => x.Id).GeneratedBy.Increment();

      Map(x => x.CreationDateTime).Not.Nullable();
    }
  }

  public class User_Map : BaseObject_Map<User>
  {
    public User_Map()
    {
      Map(x => x.Login).Not.Nullable().Length(100);
      Map(x => x.PasswordHash).Not.Nullable().Length(200);
      Map(x => x.Name).Nullable().Length(100);
      Map(x => x.Surname).Nullable().Length(100);
      Map(x => x.Patronymic).Nullable().Length(100);
      Map(x => x.Email).Nullable().Length(100);
      Map(x => x.PhotoRelativePath).Nullable().Length(200);
      HasMany<Payment>(x => x.Payments).KeyColumn("UserId").Inverse().Cascade.All();
    }
  }

  #region Платеж
  public abstract class AbstractPayment_Map<TPayment> : BaseObject_Map<TPayment>
    where TPayment : Payment
  {
    public AbstractPayment_Map()
    {
      References(x => x.User).Column("UserId").Not.Nullable().Cascade.SaveUpdate();
      Map(x => x.RealMoneyAmount).Nullable();
    }
  }

  public class Payment_Map : AbstractPayment_Map<Payment>
  {
    public Payment_Map()
    {
    }
  }

  public class BuyingMyCryptCheckPayment_Map : AbstractPayment_Map<BuyingMyCryptCheckPayment>
  {
    public BuyingMyCryptCheckPayment_Map() : base()
    {
      References(x => x.BuyingMyCryptRequest).Column("BuyingMyCryptRequestId").Not.Nullable();
      Map(x => x.State).CustomType<BuyingMyCryptCheckPaymentState>();
    }
  }
  #endregion

  public class AddMyCryptTransaction_Map : BaseObject_Map<AddMyCryptTransaction>
  {
    public AddMyCryptTransaction_Map()
    {
      Map(x => x.MyCryptCount).Not.Nullable();
      Map(x => x.Comment).Nullable().Length(5000);
      References(x => x.User).Column("UserId");
      Map(x => x.ImageRelativePath).Nullable().Length(1000);
    }
  }

  public class BiddingParticipateApplication_Map : BaseObject_Map<BiddingParticipateApplication>
  {
    public BiddingParticipateApplication_Map()
    {
      References(x => x.Seller).Not.Nullable().Column("SaylerId");
      Map(x => x.MyCryptCount).Not.Nullable();
      Map(x => x.State).CustomType<BiddingParticipateApplicationState>();
      HasMany<BuyingMyCryptRequest>(x => x.BuyingMyCryptRequests).KeyColumn("BiddingParticipateApplicationId").Inverse().Cascade.All();
    }
  }

  public class BuyingMyCryptRequest_Map : BaseObject_Map<BuyingMyCryptRequest>
  {
    public BuyingMyCryptRequest_Map()
    {
      References(x => x.SellerUser).Not.Nullable().Column("SellerUserId");
      References(x => x.Buyer).Not.Nullable().Column("BuyerId");
      References(x => x.BiddingParticipateApplication, "BiddingParticipateApplicationId").Cascade.SaveUpdate();
      Map(x => x.MyCryptCount).Not.Nullable();
      Map(x => x.Comment).Nullable().Length(3000);
      Map(x => x.State).CustomType<BuyingMyCryptRequestState>();
      HasOne(x => x.CheckPayment).Cascade.All();
    }
  }
  #endregion

  #region Event listeners
  public class PreInsertEvent : IPreInsertEventListener
  {
    public bool OnPreInsert(NHibernate.Event.PreInsertEvent @event)
    {
      BaseObject baseObject = (@event.Entity as BaseObject);

      if (baseObject == null)
        return false;

      #region Задаю время создания
      int createdDateTimeIndex = Array.IndexOf(@event.Persister.PropertyNames, "CreationDateTime");

      @event.State[createdDateTimeIndex] = DateTime.Now.ToUniversalTime();
      #endregion

      return false;
    }
  }
  #endregion
}
