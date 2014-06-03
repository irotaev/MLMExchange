using Logic;
using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Models.PaymentSystem
{
  /// <summary>
  /// Модель счета
  /// </summary>
  public class BillModel : AbstractDataModel<D_Bill, BillModel>
  {
    /// <summary>
    /// Кто выставил счет
    /// </summary>
    public UserModel User { get; set; }
    /// <summary>
    /// Количество денег по счету
    /// </summary>
    public decimal? MoneyAmount { get; set; }
    /// <summary>
    /// Состояние счета
    /// </summary>
    public BillPaymentState BillPaymentState { get; set; }
    
    public override BillModel Bind(D_Bill @object)
    { 
      if (@object == null)
        throw new ArgumentNullException("object");

      base.Bind(@object);

      User = new UserModel().Bind(@object.Payer);
      MoneyAmount = @object.MoneyAmount;
      BillPaymentState = @object.PaymentState;

      return this;
    }

    public override D_Bill UnBind(D_Bill @object = null)
    {
      throw new NotImplementedException();
    }
  }
}