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
    where TDataRole : D_AbstractRole, new()
    where TModel : class, IAbstractRoleModel
  {
    /// <summary>
    /// Пользователь, к которому прекреплена роль.
    /// </summary>
    public UserModel User
    {
      get
      {
        if (_Object == null)
          throw new BindNotCallException<D_AbstractRole>();

        return new UserModel().Bind(_Object.User);
      }
    }
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
