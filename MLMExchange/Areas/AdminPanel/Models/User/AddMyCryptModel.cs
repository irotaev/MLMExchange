using DataAnnotationsExtensions;
using Logic;
using MLMExchange.Lib.DataValidation;
using MLMExchange.Models;
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
  public class AddMyCryptModel : AbstractDataModel, IDataBinding<AddMyCryptTransaction>
  {
    /// <summary>
    /// Количество пополняемых my crypt
    /// </summary>
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public int MyCryptCount { get; set; }
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
    /// С условиями согласен
    /// </summary>
    [Boolean(ValidState = true, ErrorMessageResourceName = "FieldFilledInvalid_Boolean", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]    
    public bool IsTermsAgreed { get; set; }

    public void Bind(AddMyCryptTransaction @object)
    {
      if (@object == null)
        throw new ArgumentNullException("object");

      Id = @object.Id;
      MyCryptCount = @object.MyCryptCount;
      Comment = @object.Comment;
      ImageRelativePath = @object.ImageRelativePath;
    }

    public AddMyCryptTransaction UnBind(AddMyCryptTransaction @object = null)
    {
      if (@object == null)
        @object = new AddMyCryptTransaction();

      @object.MyCryptCount = MyCryptCount;
      @object.Comment = Comment;
      @object.ImageRelativePath = ImageRelativePath;

      return @object;
    }
  }
}
