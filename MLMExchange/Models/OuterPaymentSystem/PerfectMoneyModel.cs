using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MLMExchange.Models.OuterPaymentSystem
{
  public class PerfectMoneyModel : AbstractModel
  {    
    public string PayeeAccount { get; set; }
    public string PayeeName { get; set; }
    public string PaymentId { get; set; }
    public decimal PaymentAmount { get; set; }
    public string PaymentUnit { get; set; }
    public string StatusUrl { get; set; }
    public string PaymentUrl { get; set; }
    public string PaymentUrlMethod { get; set; }
    public string NoPaymentUrl { get; set; }
    public string NoPaymentUrlMethod { get; set; }
    public string SuggestedMemo { get; set; }

    /// <summary>
    /// Дополнительные поля
    /// </summary>
    public Dictionary<string, string> AdditionalFields { get; set; }
  }
}