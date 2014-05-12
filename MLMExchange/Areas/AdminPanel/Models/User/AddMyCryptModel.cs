using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Lib.DataValidation;
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
  /// Модель пополнения my crypt
  /// </summary>
  public class AddMyCryptModel : AbstractDataModel, IDataBinding<D_AddMyCryptTransaction, AddMyCryptModel>
  {
    /// <summary>
    /// Количество пополняемых my crypt
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public long? MyCryptCount { get; set; }

    /// <summary>
    /// Комментарии
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Картинка, подтверждающая пополнение my crypt
    /// </summary>
    public HttpPostedFileBase Image { get; set; }
    [HiddenInput(DisplayValue = false)]
    public string ImageRelativePath { get; set; }

    /// <summary>
    /// Инициатор. Пользователь
    /// </summary>
    public UserModel Initiator { get; private set; }

    /// <summary>
    /// С условиями согласен
    /// </summary>
    [Boolean(ValidState = true, ErrorMessageResourceName = "FieldFilledInvalid_Boolean", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]    
    public bool IsTermsAgreed { get; set; }

    public AddMyCryptModel Bind(D_AddMyCryptTransaction @object)
    {
      if (@object == null)
        throw new ArgumentNullException("object");

      base.Bind(@object);
      
      MyCryptCount = @object.MyCryptCount;
      Comment = @object.Comment;
      ImageRelativePath = @object.ImageRelativePath;
      Initiator = new UserModel().Bind(@object.User);

      return this;
    }

    public D_AddMyCryptTransaction UnBind(D_AddMyCryptTransaction @object = null)
    {
      if (@object == null)
        @object = new D_AddMyCryptTransaction();

      base.UnBind(@object);

      if (MyCryptCount == null)
        throw new UserVisible__ArgumentNullException("MyCryptCount");

      @object.MyCryptCount = MyCryptCount.Value;
      @object.Comment = Comment;
      @object.ImageRelativePath = ImageRelativePath;

      return @object;
    }
  }
}
