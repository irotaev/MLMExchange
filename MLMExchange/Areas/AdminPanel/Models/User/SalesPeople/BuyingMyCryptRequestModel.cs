using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Lib.Exception;
using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace MLMExchange.Areas.AdminPanel.Models.User.SalesPeople
{
  /// <summary>
  /// Оплата процентной ставки продавцу
  /// </summary>
  public interface IBuyingMyCryptRequest_SallerInterestRateModel
  {
    /// <summary>
    /// Значение процентной ставки продавцу
    /// </summary>
    decimal? SallerInterestRateValue { get; }
  }

  public class BuyingMyCryptRequestModel : AbstractDataModel<BuyingMyCryptRequest, BuyingMyCryptRequestModel>, IBuyingMyCryptRequest_SallerInterestRateModel
  {
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public long? MyCryptCount { get; set; }

    /// <summary>
    /// Id продавца
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public long SellerId { get; set; }

    /// <summary>
    /// Продавец
    /// </summary>
    public UserModel Seller { get; private set; }

    /// <summary>
    /// Покупатель
    /// </summary>    
    [HiddenInput(DisplayValue = false)]
    public UserModel Buyer { get; set; }

    /// <summary>
    /// Комментарии
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Статус запроса
    /// </summary>
    public BuyingMyCryptRequestState State { get; set; }

    /// <summary>
    /// Id торговой сессии
    /// </summary>
    public long? TradeSessionId { get; set; }

    /// <summary>
    /// Оплачен ли счет по проверочному платежу
    /// </summary>
    [HiddenInput(DisplayValue = false)]
    public bool IsCheckBillPaid { get; set; }

    /// <summary>
    /// Необходимо ли довнести деньги по счету
    /// </summary>
    public bool IsSellerInterestRatePaid_NeedSubstantialMoney { get; private set; }

    /// <summary>
    /// Оплачен ли счет по комиссионному платежу пиродавца
    /// </summary>
    [HiddenInput(DisplayValue = false)]
    public bool IsSellerInterestRatePaid { get; private set; }

    /// <summary>
    /// Значение процентной ставки продавцу
    /// </summary>
    public decimal? SallerInterestRateValue { get; private set; }

    /// <summary>
    /// Локализированное имя состояния заявки
    /// </summary>
    public string LocalisedStateDisplayName { get { return State.ToLocal(); } }

    public override BuyingMyCryptRequestModel Bind(BuyingMyCryptRequest @object)
    {
      base.Bind(@object);

      MyCryptCount = @object.MyCryptCount;
      SellerId = @object.SellerUser.Id;
      Seller = new UserModel().Bind(@object.SellerUser);
      Comment = @object.Comment;
      Buyer = new UserModel().Bind(@object.Buyer);
      State = @object.State;

      if (@object.TradingSession != null)
      {
        IsCheckBillPaid = @object.TradingSession.CheckBill.PaymentState == BillPaymentState.Paid;

        IsSellerInterestRatePaid = @object.TradingSession.SallerInterestRateBill.PaymentState == BillPaymentState.Paid;
        IsSellerInterestRatePaid_NeedSubstantialMoney = @object.TradingSession.SallerInterestRateBill.IsNeedSubstantialMoney;

        TradeSessionId = @object.TradingSession.Id;
      }

      SallerInterestRateValue = 145;

      return this;
    }

    public override BuyingMyCryptRequest UnBind(BuyingMyCryptRequest @object = null)
    {
      if (@object == null)
        @object = new BuyingMyCryptRequest();

      @object = base.UnBind(@object);

      if (MyCryptCount == null)
        throw new UserVisible__ArgumentNullException("MyCryptCount");

      @object.MyCryptCount = MyCryptCount.Value;

      Logic.D_User sellerUser = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>()
        .Session.QueryOver<Logic.D_User>().Where(x => x.Id == SellerId).List().FirstOrDefault();

      if (sellerUser == null)
        throw new UserVisible__WrongParametrException("SellerId");

      @object.SellerUser = sellerUser;
      @object.Comment = Comment;
      @object.Buyer = MLMExchange.Lib.CurrentSession.Default.CurrentUser;
      @object.State = BuyingMyCryptRequestState.AwaitingConfirm;

      BiddingParticipateApplication biddingApplication = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>()
        .Session.QueryOver<Logic.BiddingParticipateApplication>().Where(x => x.Seller.Id == SellerId && x.State == BiddingParticipateApplicationState.Filed).List().FirstOrDefault();

      if (biddingApplication == null)
        throw new UserVisible__WrongParametrException("SellerId");

      @object.BiddingParticipateApplication = biddingApplication;

      return @object;
    }

  }
}
