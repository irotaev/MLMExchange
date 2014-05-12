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
  /// Модель торговой сессии
  /// </summary>
  public class TradingSessionModel : AbstractDataModel<TradingSession, D_TradingSession, TradingSessionModel>, IPayYieldTradingSessionModel
  {
    private readonly List<YieldSessionPaymentAcceptor> _YieldSessionPaymentAcceptors = new List<YieldSessionPaymentAcceptor>();

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
      CheckBill = new BillModel().Bind(@object.LogicObject.CheckBill);
      SallerInterestRateBill = new BillModel().Bind(@object.LogicObject.SallerInterestRateBill);
      State = @object.LogicObject.State;
      IsYieldSessionBillsPaid = @object.IsYieldSessionBillsPaid;

      //TODO:Rtv переделать
      foreach (var bill in @object.LogicObject.YieldSessionBills)
      {
        YieldSessionPaymentAcceptor paymentAcceptor = new YieldSessionPaymentAcceptor
        {
          DefaultPaymentSystem = bill.PaymentAcceptor.PaymentSystemGroup.BankPaymentSystems.Where(bs => bs.IsDefault == true).FirstOrDefault().BankName,
          MoneyAmount = bill.MoneyAmount.Value,
          UserId = bill.PaymentAcceptor.Id,
          UserLogin = bill.PaymentAcceptor.Login
        };

        _YieldSessionPaymentAcceptors.Add(paymentAcceptor);
      }

      return this;
    }

    public override TradingSession UnBind(TradingSession @object = null)
    {
      throw new NotImplementedException();
    }

    public IList<YieldSessionPaymentAcceptor> YieldSessionPaymentAcceptors { get { return _YieldSessionPaymentAcceptors; } }
  }

  /// <summary>
  /// Пользователь, принемающий комиссионный платеж
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
  }
}