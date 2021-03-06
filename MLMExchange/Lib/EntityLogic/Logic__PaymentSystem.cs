﻿using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using NHibernate.Linq;
using MLMExchange.Lib.Exception;

namespace MLMExchange.Lib.EntityLogic
{
  public class Logic__PaymentSystem : AbstractEntityLogic
  {
    /// <summary>
    /// Получить платежную систему по Id
    /// </summary>
    /// <param name="id">Id платежной системы</param>
    /// <returns>Найденная платежная система</returns>
    public static D_PaymentSystem GetPaymentSystemByGuid(long id)
    {
      D_PaymentSystem findPaymentSystem;

      findPaymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_PaymentSystem>().Where(x => x.Id == id).FirstOrDefault();

      return findPaymentSystem;
    }
  }
}