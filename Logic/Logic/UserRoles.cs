using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using Microsoft.Practices.Unity;

namespace Logic
{
  /// <summary>
  /// Абстрактный прокси-класс для роли пользователя
  /// </summary>
  /// <typeparam name="TDataObject">Тип объекта данных для конкретной роли пользователя</typeparam>
  public abstract class AbstractUserRole<TDataObject> : AbstractLogicObject<TDataObject>
    where TDataObject : D_AbstractRole
  {
    public AbstractUserRole(TDataObject dataObject) : base(dataObject) { }
  }

  /// <summary>
  /// Прокси-класс для роли пользователя
  /// </summary>
  public class UserRole : AbstractUserRole<D_UserRole>
  {
    public UserRole(D_UserRole d_userRole) : base(d_userRole) { }

    public static explicit operator UserRole(D_UserRole dataBaseObject)
    {
      return new UserRole(dataBaseObject);
    }

    /// <summary>
    /// Посчитать общий профит для реферера данной роли
    /// </summary>
    /// <returns>Общий профит</returns>
    public decimal CalculateTotalRefererProfit()
    {
      decimal profitCount = 0m;

      if (LogicObject.RefererProfits.Count == 0)
        return profitCount;

      foreach(var profit in LogicObject.RefererProfits)
      {
        profitCount += profit.RefererProfit;
      }

      profitCount = Bill.RoundBillMoneyAmount(profitCount);

      return profitCount;
    }
  }

  public class UserRoleList
  {
    //TODO:Кузя перенести. Этот метод не относится к списку ролей
    /// <summary>
    /// Получить пользователей
    /// </summary>
    /// <param name="start">Начальный номер</param>
    /// <param name="limit">Число пользователей</param>
    /// <param name="totalCount">Общее количество пользователей</param>
    /// <returns>Список пользователей</returns>
    public static List<TDataModel> GetUsers<TDataModel>(int start, int limit, out int totalCount)
    {
      List<TDataModel> userList = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<TDataModel>().Skip(start).Take(limit).ToList();

      totalCount = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<TDataModel>().Count();

      return userList;
    }
  }
}
