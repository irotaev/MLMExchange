using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
  /// <summary>
  /// Прокси-объект счета.
  /// Используется для реализации наследования
  /// </summary>
  /// <typeparam name="TInheritObject">Объект данных, который является реализацией данных для
  /// текущего прокси-объекта, который наследует данный объект</typeparam>
  public class Bill<TInheritObject> : AbstractLogicObject<TInheritObject>
    where TInheritObject : D_Bill
  {
    public Bill(TInheritObject d_dataObject) : base(d_dataObject) { }

    /// <summary>
    /// Добавить платеж
    /// </summary>
    /// <param name="payment">Платеж</param>
    public virtual void AddPayment(Payment payment)
    {
      _LogicObject.GetPaymentList().Add(payment);
    }
  }

  public class Bill : Bill<D_Bill>
  {
    public Bill(D_Bill d_dataObject) : base(d_dataObject) { }

    public static explicit operator Bill(D_Bill dataBaseObject)
    {
      return new Bill(dataBaseObject);
    }
  }
}
