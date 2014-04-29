﻿using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Areas.AdminPanel.Models.User.SalesPeople;
using MLMExchange.Lib.Exception;
using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

    public BiddingParticipateApplicationModel Bind(BiddingParticipateApplication @object)
    {
      base.Bind(@object);

      MyCryptCount = @object.MyCryptCount;

      UserModel userModel = new UserModel();
      userModel.Bind(@object.Seller);

      Seller = userModel;

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
