using Logic;
using MLMExchange.Areas.AdminPanel.Models.PaymentSystem;
using MLMExchange.Areas.AdminPanel.Models.User;
using MLMExchange.Areas.AdminPanel.Models.User.SalesPeople;
using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Models
{
  /// <summary>
  /// Модель оплаты доходности торговой сессии
  /// </summary>
  public interface IPayYieldTradingSessionModel
  {
    IList<YieldSessionPaymentAcceptor> YieldSessionPaymentAcceptors { get; }
  }

  /// <summary>
  /// Модель торговой сессии, отображающая состояние "NeedProfit"
  /// </summary>
  public interface INeedPaymentProfitModel
  {
    /// <summary>
    /// Счета, для обеспечения прибыли торговой сессии
    /// </summary>
    IEnumerable<BillModel> NeedProfitBills { get; }
  }

  /// <summary>
  /// Модель торговой сессии
  /// </summary>
  public class TradingSessionModel : AbstractDataModel<TradingSession, D_TradingSession, TradingSessionModel>, IPayYieldTradingSessionModel, INeedPaymentProfitModel
  {
    private readonly List<YieldSessionPaymentAcceptor> _YieldSessionPaymentAcceptors = new List<YieldSessionPaymentAcceptor>();
    private readonly List<TradingSessionCurrentUserType> _CurrentUserTypes = new List<TradingSessionCurrentUserType>();

    /// <summary>
    /// Список типов текущего пользователя, по отношению к торговой сессии
    /// </summary>
    public IEnumerable<TradingSessionCurrentUserType> CurrentUserTypes { get { return _CurrentUserTypes.ToList(); } }

    /// <summary>
    /// Заявка на продажу my-crypt
    /// </summary>
    public BiddingParticipateApplicationModel BiddingParticipateApplication { get; set; }
    /// <summary>
    /// Заявка на покупку my-crypt
    /// </summary>
    public BuyingMyCryptRequestModel BuyingMyCryptRequest { get; set; }
    /// <summary>
    /// Проверочный счет
    /// </summary>
    public BillModel CheckBill { get; set; }
    /// <summary>
    /// Счет на комисионный сбор продавца
    /// </summary>
    public BillModel SallerInterestRateBill { get; set; }
    /// <summary>
    /// Статус торговой сессии
    /// </summary>
    public TradingSessionStatus State { get; set; }
    /// <summary>
    /// Дата и время когда можно будет закрыть сессию
    /// </summary>
    public DateTime? ClosingSessionDateTime { get; set; }

    /// <summary>
    /// Оплачены ли счета доходности торговой сессии
    /// </summary>
    public bool IsYieldSessionBillsPaid { get; private set; }

    public override TradingSessionModel Bind(TradingSession @object)
    {
      if (@object == null)
        throw new ArgumentNullException("object");

      base.Bind(@object);

      BiddingParticipateApplication = new BiddingParticipateApplicationModel().Bind(@object.LogicObject.BiddingParticipateApplication);
      BuyingMyCryptRequest = new BuyingMyCryptRequestModel().Bind(@object.LogicObject.BuyingMyCryptRequest);
      CheckBill = new BillModel().Bind(((Bill)@object.LogicObject.CheckBill));
      SallerInterestRateBill = new BillModel().Bind(((Bill)@object.LogicObject.SallerInterestRateBill));
      State = @object.LogicObject.State;
      IsYieldSessionBillsPaid = @object.IsYieldSessionBillsPaid;
      ClosingSessionDateTime = @object.LogicObject.ClosingSessionDateTime;

      //TODO:Rtv переделать
      foreach (var bill in @object.LogicObject.YieldSessionBills.Where(x => x.PaymentState == BillPaymentState.WaitingPayment))
      {
        Logic.PaymentSystem defaultPaymentSystem = ((PaymentSystemGroup)bill.PaymentAcceptor.PaymentSystemGroup).GetDefaultPaymentSystem();

        YieldSessionPaymentAcceptor paymentAcceptor = new YieldSessionPaymentAcceptor
        {
          DefaultPaymentSystem = defaultPaymentSystem != null ? defaultPaymentSystem.GetType().Name : "TODO: доработать",
          MoneyAmount = bill.MoneyAmount,
          UserId = bill.PaymentAcceptor.Id,
          UserLogin = bill.PaymentAcceptor.Login,
          YieldTradingSessionBillId = bill.Id
        };

        _YieldSessionPaymentAcceptors.Add(paymentAcceptor);
      }

      #region Задание типа текущего пользователя, по отношению к торговой сессии
      if (@object.LogicObject.BuyingMyCryptRequest != null && @object.LogicObject.BuyingMyCryptRequest.Buyer.Id == MLMExchange.Lib.CurrentSession.Default.CurrentUser.Id)
        _CurrentUserTypes.Add(TradingSessionCurrentUserType.Buyer);

      if (@object.LogicObject.BuyingMyCryptRequest != null && @object.LogicObject.BuyingMyCryptRequest.SellerUser.Id == MLMExchange.Lib.CurrentSession.Default.CurrentUser.Id)
        _CurrentUserTypes.Add(TradingSessionCurrentUserType.Seller);
      #endregion

      return this;
    }

    public override TradingSession UnBind(TradingSession @object = null)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Список пользователей, принимающих комиссионный платеж
    /// </summary>
    public IList<YieldSessionPaymentAcceptor> YieldSessionPaymentAcceptors { get { return _YieldSessionPaymentAcceptors; } }

    private IEnumerable<BillModel> _NeedProfitBills;
    public IEnumerable<BillModel> NeedProfitBills
    {
      get
      {
        if (_Object.LogicObject.State == TradingSessionStatus.NeedProfit || _Object.LogicObject.State == TradingSessionStatus.ProfitConfirmation)
        {
          if (_NeedProfitBills == null)
          {
            _NeedProfitBills = _Object.GetNeedPaymentBills().Where(x => x.LogicObject.PaymentState == BillPaymentState.EnoughMoney).Select(x => new BillModel().Bind((Bill)x));
          }

          return _NeedProfitBills;
        }
        else
        {
          return null;
        }
      }
    }
  }

  /// <summary>
  /// Пользователь, принимающий комиссионный платеж
  /// </summary>
  public class YieldSessionPaymentAcceptor
  {
    public string UserLogin { get; set; }
    public long UserId { get; set; }
    /// <summary>
    /// Количество денег, которые необходимо заплатить
    /// </summary>
    public decimal MoneyAmount { get; set; }
    /// <summary>
    /// Дефолтная платежная система
    /// </summary>
    public string DefaultPaymentSystem { get; set; }
    public long YieldTradingSessionBillId { get; set; }
  }

  /// <summary>
  /// Тип текущего пользователя, по отношению к торговой сессии
  /// </summary>
  public enum TradingSessionCurrentUserType
  {
    /// <summary>
    /// Покупатель (тот, кто подал заявку на покупку my-crypt)
    /// </summary>
    Buyer = 1,
    /// <summary>
    /// Продавец (тот, кто подал заявку на продажу my-crypt)
    /// </summary>
    Seller = 2
  }
}