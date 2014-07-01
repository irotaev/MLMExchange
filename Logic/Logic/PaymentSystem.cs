using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
  /// <summary>
  /// Объект прокси-логики платежной системы
  /// </summary>
  public class PaymentSystem : AbstractLogicObject<D_PaymentSystem>
  {
    public PaymentSystem(D_PaymentSystem d_object) : base(d_object) { }

    public static explicit operator PaymentSystem(D_PaymentSystem dataBaseObject)
    {
      return new PaymentSystem(dataBaseObject);
    }

    /// <summary>
    /// Получить удобочитаемое имя платежной системы
    /// </summary>
    /// <param name="paymentSystem">Платежная система, для колторой получить имя</param>
    /// <returns>Имя платежной системы</returns>
    public static string GetDisplayName(PaymentSystem paymentSystem)
    {
      if (paymentSystem == null)
        throw new ArgumentNullException("paymentSystem");

      if (paymentSystem.LogicObject is D_BankPaymentSystem)
      {
        return Logic.Properties.GeneralResources.BankPaymentSystem;
      }
      else if (paymentSystem.LogicObject is D_ElectronicPaymentSystem)
      {
        return Logic.Properties.GeneralResources.ElectronicPaymentSystem;
      }
      else
      {
        return Logic.Properties.GeneralResources.PaymentSystem;
      }
    }
  }
}
