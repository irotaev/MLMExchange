using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Areas.AdminPanel.Models.PaymentSystem;
using MLMExchange.Lib.Exception;
using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.Unity;
using NHibernate.Linq;
using MLMExchange.Areas.AdminPanel.Models.User.SalesPeople;
using System.Collections.Generic;

namespace MLMExchange.Areas.AdminPanel.Models.User
{
  /// <summary>
  /// Модель заявки на участие в торгах
  /// </summary>
  public class BiddingParticipateApplicationModel : AbstractDataModel, IDataBinding<BiddingParticipateApplication, BiddingParticipateApplicationModel>
  {
    /// <summary>
    /// Количество my-crypt для подачи в заявку
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public long? MyCryptCount { get; set; }

    public UserModel Seller { get; set; }

    /// <summary>
    /// Состояние заявки
    /// </summary>
    public BiddingParticipateApplicationState State { get; set; }
    public PaymentSystemGroupModel PaymentSystemGroupModel { get; set; }

    public BiddingParticipateApplicationModel Bind(BiddingParticipateApplication @object)
    {
      base.Bind(@object);
      MyCryptCount = @object.MyCryptCount;

      State = @object.State;

      Seller = new UserModel().Bind(@object.Seller);
      PaymentSystemGroupModel = new PaymentSystemGroupModel().Bind((PaymentSystemGroup)@object.Seller.PaymentSystemGroup); 

      return this;
    }

    public BiddingParticipateApplication UnBind(BiddingParticipateApplication @object = null)
    {
      if (@object == null)
        @object = new BiddingParticipateApplication();

      base.UnBind(@object);

      if (MyCryptCount == null)
        throw new UserVisible__ArgumentNullException("MyCryptCount");

      @object.MyCryptCount = MyCryptCount.Value;
      @object.Seller = MLMExchange.Lib.CurrentSession.Default.CurrentUser;
      @object.State = BiddingParticipateApplicationState.Filed;

      PaymentSystemGroupModel PaymentSystemModel = new PaymentSystemGroupModel();
      PaymentSystemGroupModel = PaymentSystemModel;

      return @object;
    }
  }

  /// <summary>
  /// Модель заявки в состоянии покупатель нашелся
  /// </summary>
  public class BiddingParticipateApplicationBuyerFoundModel : BaseModel
  {
    /// <summary>
    /// Заявки на покупку
    /// </summary>
    public List<BuyingMyCryptRequestModel> BuyRequests { get; set; }
  }

  /// <summary>
  /// Модель заявки на продажу в состоянии покупатель принят
  /// </summary>
  public class BiddingParticipateApplicationAcceptedModel : BaseModel
  {
    /// <summary>
    /// Покупатель
    /// </summary>
    public UserModel Buyer { get; set; }
    /// <summary>
    /// Необходимо ли довнесение денег покупателем по комиссионному счету торговой сессии, выставленному продавцом
    /// </summary>
    public bool IsSellerInterestRate_NeedSubstantialMoney { get; set; }
    /// <summary>
    /// Id текущей торговой сессии
    /// </summary>
    public long TradingSessionId { get; set; }
    /// <summary>
    /// Id платежей продавцу MC
    /// </summary>
    public IEnumerable<long> PaymentIds { get; set; }
  }

  #region Состояние заявки на участие в торгах
  public interface IBaseBiddingParticipateApplicationModel
  {
    ApplicationState State { get; }
  }

  public interface IBiddingParticipateApplicationNotFiledModel : IBaseBiddingParticipateApplicationModel
  {
    BiddingParticipateApplicationModel BiddingParticipateApplicationModel { get; }
  }

  public interface IBiddingParticipateApplicationFiledModel : IBaseBiddingParticipateApplicationModel
  {
  }

  public interface IBiddingParticipateApplicationBuyerFoundModel : IBaseBiddingParticipateApplicationModel
  {
    BiddingParticipateApplicationBuyerFoundModel BiddingParticipateApplicationBuyerFoundModel { get; }
  }

  public interface IBiddingParticipateApplicationDeniedModel : IBaseBiddingParticipateApplicationModel
  {
  }

  public interface IBiddingParticipateApplicationAcceptedModel : IBaseBiddingParticipateApplicationModel
  {
    BiddingParticipateApplicationAcceptedModel BiddingParticipateApplicationAcceptedModel { get; }
  }

  /// <summary>
  /// Модель отображает все возмоджные состояния заявки на участие в торгах
  /// </summary>
  public class BiddingParticipateApplicationStateModel :
    IBiddingParticipateApplicationNotFiledModel,
    IBiddingParticipateApplicationFiledModel,
    IBiddingParticipateApplicationBuyerFoundModel,
    IBiddingParticipateApplicationDeniedModel,
    IBiddingParticipateApplicationAcceptedModel
  {
    public BiddingParticipateApplicationModel BiddingParticipateApplicationModel { get; set; }

    public BiddingParticipateApplicationBuyerFoundModel BiddingParticipateApplicationBuyerFoundModel { get; set; }

    public ApplicationState State { get; set; }

    public BiddingParticipateApplicationAcceptedModel BiddingParticipateApplicationAcceptedModel { get; set; }
  }

  public enum ApplicationState : int
  {
    NA = 0,
    /// <summary>
    /// Не подана
    /// </summary>
    NotFiled = 1,
    /// <summary>
    /// Ожидает покупателей
    /// </summary>
    ExpectsBuyers = 2,
    /// <summary>
    /// Покупатель найден
    /// </summary>
    BuyerFound = 3,
    /// <summary>
    /// Отклонена
    /// </summary>
    Denied = 4,
    /// <summary>
    /// Принята
    /// </summary>
    Accepted = 5
  }
  #endregion
}
