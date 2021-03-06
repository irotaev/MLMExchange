﻿using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Areas.AdminPanel.Models.PaymentSystem;
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
  /// Базовый класс для всех платежных систем
  /// </summary>
  /// <typeparam name="TDataObject">Объект данных соответствующей платежной системы</typeparam>
  /// <typeparam name="TModel">Модель платежной системы</typeparam>
  public abstract class AbstractPaymentSystemModel<TDataObject, TModel> : AbstractDataModel<TDataObject, TModel>
    where TDataObject : D_PaymentSystem, new()
    where TModel : AbstractModel
  {
  }

  /// <summary>
  /// Базовый класс для всех платежных систем
  /// </summary>
  /// <typeparam name="TDataObject">Объект данных соответствующей платежной системы</typeparam>
  /// <typeparam name="TLogicObject">Объект логики (прокси-объект) соответствующей платежной системы</typeparam>
  /// <typeparam name="TModel">Модель платежной системы</typeparam>
  public abstract class AbstractPaymentSystemModel<TLogicObject, TDataObject, TModel> : AbstractDataModel<TLogicObject, TDataObject, TModel>
    where TLogicObject : AbstractLogicObject<TDataObject>
    where TDataObject : D_PaymentSystem, new()
    where TModel : AbstractModel
  {
  }

  /// <summary>
  /// Базовый объект платежной системы
  /// </summary>
  public class BasePaymentSystemModel : AbstractPaymentSystemModel<Logic.PaymentSystem, D_PaymentSystem, BasePaymentSystemModel>
  {
  }

  /// <summary>
  /// Модель платежной системы банк
  /// </summary>
  public class BankPaymentSystemModel : AbstractPaymentSystemModel<D_BankPaymentSystem, BankPaymentSystemModel>
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
    [StringLength(10, MinimumLength = 10, ErrorMessageResourceName = "FieldCharCountNotRequired_10", ErrorMessageResourceType = typeof(MLMExchange.Properties.PrivateResource))]
    [RegularExpression("^[0-9]+$", ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string INN { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [StringLength(9, MinimumLength = 9, ErrorMessageResourceName = "FieldCharCountNotRequired_9", ErrorMessageResourceType = typeof(MLMExchange.Properties.PrivateResource))]
    [RegularExpression("^[0-9]+$", ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string KPP { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [StringLength(20, MinimumLength = 20, ErrorMessageResourceName = "FieldCharCountNotRequired_20", ErrorMessageResourceType = typeof(MLMExchange.Properties.PrivateResource))]
    [RegularExpression("^[0-9]+$", ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string CurrentAccount { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [StringLength(9, MinimumLength = 9, ErrorMessageResourceName = "FieldCharCountNotRequired_9", ErrorMessageResourceType = typeof(MLMExchange.Properties.PrivateResource))]
    [RegularExpression("^[0-9]+$", ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string BIK { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [StringLength(18, MinimumLength = 16, ErrorMessageResourceName = "FieldRangeCharCountNotRequired_16_18", ErrorMessageResourceType = typeof(MLMExchange.Properties.PrivateResource))]
    [RegularExpression("^[0-9]+$", ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string CardNumber { get; set; }

    /// <summary>
    /// Является ли данная система дефолтной
    /// </summary>
    [HiddenInput(DisplayValue = false)]
    public bool IsDefault { get; set; }

    public override BankPaymentSystemModel Bind(D_BankPaymentSystem @object)
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
      CardNumber = @object.CardNumber;

      IsDefault = @object.IsDefault;

      return this;
    }

    public override D_BankPaymentSystem UnBind(D_BankPaymentSystem @object = null)
    {
      if (@object == null)
        @object = new D_BankPaymentSystem();

      base.UnBind(@object);

      @object.UserName = UserName;
      @object.UserSurname = UserSurname;
      @object.UserPatronymic = UserPatronymic;
      @object.BankName = BankName;

      if (String.IsNullOrWhiteSpace(INN))
        throw new Logic.Lib.UserVisible__ArgumentNullException("INN");

      @object.INN = INN;

      if (String.IsNullOrWhiteSpace(KPP))
        throw new Logic.Lib.UserVisible__ArgumentNullException("KPP");

      @object.KPP = KPP;

      if (String.IsNullOrWhiteSpace(CurrentAccount))
        throw new Logic.Lib.UserVisible__ArgumentNullException("CurrentAccount");

      @object.CurrentAccount = CurrentAccount;

      if (String.IsNullOrWhiteSpace(BIK))
        throw new Logic.Lib.UserVisible__ArgumentNullException("BIK");

      @object.BIK = BIK;

      if (String.IsNullOrWhiteSpace(CardNumber))
        throw new Logic.Lib.UserVisible__ArgumentNullException("CardNumber");

      @object.CardNumber = CardNumber;

      return @object;
    }
  }
  public class ElectronicPaymentSystemModel : AbstractPaymentSystemModel<D_ElectronicPaymentSystem, ElectronicPaymentSystemModel>
  {
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string ElectronicName { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string PurseNumber { get; set; }

    /// <summary>
    /// Является ли данная система дефолтной
    /// </summary>
    [HiddenInput(DisplayValue = false)]
    public bool IsDefault { get; set; }

    public override ElectronicPaymentSystemModel Bind(D_ElectronicPaymentSystem @object)
    {
      if (@object == null)
        throw new ArgumentNullException("@object");

      base.Bind(@object);

      ElectronicName = @object.ElectronicName;
      PurseNumber = @object.PurseNumber;

      IsDefault = @object.IsDefault;

      return this;
    }

    public override D_ElectronicPaymentSystem UnBind(D_ElectronicPaymentSystem @object = null)
    {
      if (@object == null)
        @object = new D_ElectronicPaymentSystem();

      base.UnBind(@object);

      @object.ElectronicName = ElectronicName;

      if (String.IsNullOrWhiteSpace(PurseNumber))
        throw new Logic.Lib.UserVisible__ArgumentNullException("PurseNumber");

      @object.PurseNumber = PurseNumber;

      return @object;
    }
  }
}