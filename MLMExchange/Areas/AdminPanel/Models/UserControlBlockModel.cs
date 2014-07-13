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
    /// Имеет ли пользователь роль пользователя системы.
    /// Если не имеет то для него нет никакого смысла выводить блок контрольной панели пользователя.
    /// </summary>
    public bool IsHasUserRole
    {
      get
      {
        if (_User == null)
          throw new BindNotCallException<Logic.User>();

        return CurrentUser.UserRoles.FirstOrDefault(x => (x as Logic.D_UserRole) != null) != null;
      }
    }

    /// <summary>
    /// Id роли пользователя
    /// </summary>
    public long? UserRoleId
    {
      get
      {
        if (_User == null)
          throw new BindNotCallException<Logic.User>();

        Logic.D_UserRole userRole = CurrentUser.UserRoles.FirstOrDefault(x => (x as Logic.D_UserRole) != null) as Logic.D_UserRole;

        if (userRole == null)
        {
          return null;
        }
        else
        {
          return userRole.Id;
        }
      }
    }

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


    public decimal ReferalProfit
    {
      get
      {
        //TODO:Rtv Доделать получение прибыли рефералов
        return 0;
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
