using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Lib.Exception;
using MLMExchange.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Areas.AdminPanel.Models.PaymentSystem
{
  /// <summary>
  /// Модель платежной системы банк
  /// </summary>
  public class BankPaymentSystemModel : AbstractDataModel<BankPaymentSystem, BankPaymentSystemModel>
  {
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string UserName { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string UserSurname { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string UserPatronymic { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string BankName { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string INN { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string KPP { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string CurrentAccount { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string BIK { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string CorrespondentAccount { get; set; }

    /// <summary>
    /// Является ли данная система дефолтной
    /// </summary>
    [HiddenInput(DisplayValue = false)]
    public bool IsDefault { get; set; }

    public override BankPaymentSystemModel Bind(BankPaymentSystem @object)
    {
      if (@object == null)
        throw new ArgumentNullException("object");

      base.Bind(@object);

      UserName = @object.UserName;
      UserSurname = @object.UserSurname;
      UserPatronymic = @object.UserPatronymic;
      BankName = @object.BankName;
      INN = @object.INN;
      KPP = @object.KPP;
      CurrentAccount = @object.CurrentAccount;
      BIK = @object.BIK;
      CorrespondentAccount = @object.CorrespondentAccount;

      IsDefault = @object.IsDefault;

      return this;
    }

    public override BankPaymentSystem UnBind(BankPaymentSystem @object = null)
    {
      if (@object == null)
        @object = new BankPaymentSystem();

      base.UnBind(@object);

      @object.UserName = UserName;
      @object.UserSurname = UserSurname;
      @object.UserPatronymic = UserPatronymic;
      @object.BankName = BankName;

      if (INN == null)
        throw new UserVisible__ArgumentNullException("INN");

      @object.INN = INN.Value;

      if (KPP == null)
        throw new UserVisible__ArgumentNullException("KPP");

      @object.KPP = KPP.Value;

      if (CurrentAccount == null)
        throw new UserVisible__ArgumentNullException("CurrentAccount");

      @object.CurrentAccount = CurrentAccount.Value;

      if (BIK == null)
        throw new UserVisible__ArgumentNullException("BIK");

      @object.BIK = BIK.Value;

      if (CorrespondentAccount == null)
        throw new UserVisible__ArgumentNullException("CorrespondentAccount");

      @object.CorrespondentAccount = CorrespondentAccount.Value;

      return @object;
    }
  }
}