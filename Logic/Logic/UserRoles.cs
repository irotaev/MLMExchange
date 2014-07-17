using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

      return profitCount;
    }
  }
}
