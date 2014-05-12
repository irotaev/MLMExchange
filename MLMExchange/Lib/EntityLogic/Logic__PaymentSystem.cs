using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using Logic;
using NHibernate.Linq;
using MLMExchange.Lib.Exception;

namespace MLMExchange.Lib.EntityLogic
{
  public class Logic__PaymentSystem : AbstractEntityLogic
  {
    /// <summary>
    /// Получить платежную систему по Guid
    /// </summary>
    /// <param name="guid">Guid платежной системы</param>
    /// <returns>Найденная платежная система</returns>
    public static PaymentSystem GetPaymentSystemByGuid(string guid)
    {
      if (String.IsNullOrEmpty(guid))
        throw new ArgumentNullException("guid");

      PaymentSystem findPaymentSystem;

      findPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<BankPaymentSystem>().Where(x => x.Guid.ToString() == guid).FirstOrDefault();

      return findPaymentSystem;
    }

    /// <summary>
    /// Получить платежную систему по Guid
    /// </summary>
    /// <param name="guid">Guid платежной системы</param>
    /// <returns>Найденная платежная система</returns>
    public static PaymentSystem GetPaymentSystemByGuid(Guid guid)
    {
      return GetPaymentSystemByGuid(guid.ToString());
    }
  }
}