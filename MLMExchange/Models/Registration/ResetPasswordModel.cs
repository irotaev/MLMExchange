using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Logic;

namespace MLMExchange.Models.Registration
{
  public class ResetPasswordModel : AbstractModel
  {
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties. ResourcesA))]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessageResourceName = "FieldEmailInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Email { get; set; }
  }

  public class ResetPasswordDataModel : AbstractDataModel<D_ResetPassword, ResetPasswordDataModel>
  {
    /// <summary>
    /// Юзер, подавший заявку
    /// </summary>
    public UserModel User { get; set; }
    /// <summary>
    /// Секретный ключ
    /// </summary>
    public string HashCode { get; set; }

    public override ResetPasswordDataModel Bind(D_ResetPassword @object)
    {
      if (@object == null)
        throw new ArgumentNullException("object");

      base.Bind(@object);

      User = new UserModel().Bind(@object.User);
      HashCode = @object.HashCode;

      return this;
    }

    public override D_ResetPassword UnBind(D_ResetPassword @object = null)
    {
      if (@object == null)
        @object = new D_ResetPassword();

      base.UnBind(@object);

      @object.HashCode = HashCode;

      return @object;
    }
  }
}