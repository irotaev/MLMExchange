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
  /// Proxy-объект логики YieldSessionBill
  /// </summary>
  public class YieldSessionBill : AbstractLogicObject<D_YieldSessionBill>
  {
    public YieldSessionBill(D_YieldSessionBill d_baseObject) : base(d_baseObject) { }

    public static explicit operator YieldSessionBill(D_YieldSessionBill dataBaseObject)
    {
      return new YieldSessionBill(dataBaseObject);
    }

    public override void OnPreUpdate(NHibernate.Event.PreUpdateEvent @event)
    {
      base.OnPreUpdate(@event);
    }
  }
}
