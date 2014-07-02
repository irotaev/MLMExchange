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
  /// Объект прокси-логики платежной системы
  /// </summary>
  public class PaymentSystem : AbstractLogicObject<D_PaymentSystem>
  {
    public PaymentSystem(D_PaymentSystem d_object) : base(d_object) 
    {
      _LogicObject = base._LogicObject;
    }

    public static explicit operator PaymentSystem(D_PaymentSystem dataBaseObject)
    {
      return new PaymentSystem(dataBaseObject);
    }

    new protected D_PaymentSystem _LogicObject;
    new public D_PaymentSystem LogicObject { get { return _LogicObject; } }

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

    /// <summary>
    /// Загрузить конкретный вид платежной системы из базы.
    /// 
    /// Т.е. в LogicObject лежит D_PaymentSystem, но если на самом деле там лежит просто
    /// упакованная (урезанная) версия, например, D_BankPaymentSystem, то при загрузке 
    /// загрузится полная версия D_BankPaymentSystem и обратно упакуется в D_PaymentSystem, так
    /// что теперь ее можно кастовать к D_BankPaymentSystem
    /// </summary>
    /// <returns></returns>
    public PaymentSystem Load()
    {
      switch(_LogicObject.ToString())
      {
        case "Logic.D_BankPaymentSystem":
          _LogicObject = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<D_BankPaymentSystem>()
            .FirstOrDefault(x => x.Id == _LogicObject.Id);
          break;
        case "Logic.D_ElectronicPaymentSystem":
          _LogicObject = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session.Query<D_ElectronicPaymentSystem>()
            .FirstOrDefault(x => x.Id == _LogicObject.Id);
          break;
      }

      return this;
    }
  }
}
