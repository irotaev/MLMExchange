using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MLMExchange.Models;
using Logic;
using MLMExchange.Models.Registration;

namespace MLMExchange.Areas.AdminPanel.Models
{
  /// <summary>
  /// Интерфейс модели абстрактной роли.
  /// </summary>
  public interface IAbstractRoleModel : IAbstractModel
  {
    UserModel User { get; }
  }

  /// <summary>
  /// Модель абстрактной роли пользователя
  /// <typeparam name="TDataRole">Тип роли пользователя. Уровень данных.</typeparam>
  /// <typeparam name="TModel">Тип конкрентной модели</typeparam>
  /// </summary>
  public abstract class AbstractRoleModel<TDataRole, TModel> : AbstractDataModel<TDataRole, TModel>, IAbstractRoleModel
    where TDataRole : D_AbstractRole
    where TModel : class, IAbstractRoleModel
  {
    public AbstractRoleModel()
    {
      _LazyLoadProperties.Add("User", false);
    }

    private UserModel _User;

    /// <summary>
    /// Пользователь, к которому прекреплена роль.
    /// </summary>
    public UserModel User
    {
      get
      {
        if (_LazyLoadProperties["User"])
        {
          if (_Object == null)
            throw new BindNotCallException<D_AbstractRole>();

          _User = new UserModel().Bind(_Object.User);
          _LazyLoadProperties["User"] = false;
        }

        return _User;
      }
    }
  }

  public class BaseRoleModel : AbstractRoleModel<D_AbstractRole, BaseRoleModel>
  {
    public BaseRoleModel()
    {
      _LazyLoadProperties.Add("RoleType", false);

      _TextResources.Add("DeleteRole", MLMExchange.Properties.ResourcesA.Button_Remove);
      _TextResources.Add("Roles", Logic.Properties.GeneralResources.RoleList);
      _TextResources.Add("AddRole", MLMExchange.Properties.ResourcesA.AddRole);
      _TextResources.Add("AllRoleTypePairs", new RoleType().GetValueTypePairs());
    }

    #region RoleType
    private RoleType _RoleType;
    public RoleType RoleType
    {
      get
      {
        if (_LazyLoadProperties["RoleType"])
        {
          if (_Object == null)
            throw new BindNotCallException<D_AbstractRole>();

          _RoleType = _Object.RoleType;
          _LazyLoadProperties["RoleType"] = false;
        }

        return _RoleType;
      }

      set { _RoleType = value; }
    }
    public string RoleTypeDisplayName
    {
      get
      {
        return _RoleType.GetDisplayName();
      }
    }
    public string RoleTypeAsString
    {
      get
      {
        return Enum.GetName(typeof(RoleType), _RoleType);
      }
    }
    #endregion
  }

  /// <summary>
  /// Модель роли пользователя.
  /// </summary>
  public class UserRoleModel : AbstractRoleModel<D_UserRole, UserRoleModel>
  {
    /// <summary>
    /// Количество криптов.
    /// </summary>
    public long MyCryptCount
    {
      get
      {
        if (_Object == null)
          throw new BindNotCallException<D_UserRole>();

        return _Object.MyCryptCount;
      }
    }

    /// <summary>
    /// Рефералы.
    /// </summary>
    public IEnumerable<UserModel> ReferalUsers
    {
      get
      {
        if (_Object == null)
          throw new BindNotCallException<D_UserRole>();

        return _Object.ReferalUsers.Select(x => new UserModel().Bind(x));
      }
    }
  }

  /// <summary>
  /// Модель роли лидера.
  /// </summary>
  public class LeaderRoleModel : AbstractRoleModel<D_LeaderRole, LeaderRoleModel>
  {
  }

  /// <summary>
  /// Модель роли тестестировщика.
  /// </summary>
  public class TesterRoleModel : AbstractRoleModel<D_TesterRole, TesterRoleModel>
  {
  }

  /// <summary>
  /// Роль брокера системы.
  /// </summary>
  public class BrokerRoleModel : AbstractRoleModel<D_BrokerRole, BrokerRoleModel>
  {
  }

  /// <summary>
  /// Роль администратора системы.
  /// </summary>
  public class AdministratorRoleModel : AbstractRoleModel<D_AdministratorRole, AdministratorRoleModel>
  {
  }
}
