using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

namespace Logic
{
  /// <summary>
  /// Объект прокси-логики группы платежных систем
  /// </summary>
  public class PaymentSystemGroup : AbstractLogicObject<D_PaymentSystemGroup>
  {
    public PaymentSystemGroup(D_PaymentSystemGroup d_object) : base(d_object) { }

    public static explicit operator PaymentSystemGroup(D_PaymentSystemGroup d_object)
    {
      return new PaymentSystemGroup(d_object);
    }

    /// <summary>
    /// Получить дефолтную платежную систему для данной группы
    /// </summary>
    /// <returns>Дефолтная платежная система</returns>
    public PaymentSystem GetDefaultPaymentSystem()
    {
      D_PaymentSystem d_paymentSystem = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session
        .Query<D_PaymentSystem>().Where(x => x.PaymentSystemGroup.Id == LogicObject.Id && x.IsDefault == true).FirstOrDefault();

      if (d_paymentSystem == null)
        return null;

      return (PaymentSystem)d_paymentSystem;
    }
  }
}
