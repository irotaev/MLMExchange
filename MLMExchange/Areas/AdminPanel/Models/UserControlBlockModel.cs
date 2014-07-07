using MLMExchange.Models;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Areas.AdminPanel.Models
{
  /// <summary>
  /// Модель блока контрольной панели
  /// </summary>
  public class UserControlBlockModel : AbstractModel, ILogicDataGetter<Logic.User, UserControlBlockModel>
  {
    private Logic.User _User;

    /// <summary>
    /// Модель текущего пользователя
    /// </summary>
    public UserModel CurrentUser
    {
      get
      {
        if (_User == null)
          throw new BindNotCallException<Logic.User>();

        return new UserModel().Bind(_User.LogicObject);
      }
    }

    public UserControlBlockModel Bind(Logic.User @object)
    {
      if (@object == null)
        throw new ArgumentNullException("@object");

      _User = @object;

      return this;
    }
  }
}
