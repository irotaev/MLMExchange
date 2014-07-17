using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
  /// <summary>
  /// Прокси-класс для реферальной прибыли
  /// </summary>
  public class ReferalProfit : AbstractLogicObject<D_ReferalProfit>
  {
    public ReferalProfit(D_ReferalProfit d_dataObject) : base(d_dataObject) { }

    public static explicit operator ReferalProfit(D_ReferalProfit dataBaseObject)
    {
      return new ReferalProfit(dataBaseObject);
    }
  }
}
