using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
  /// <summary>
  /// Proxy-объект логики базового объекта
  /// </summary>
  public class BaseObject : AbstractLogicObject<D_BaseObject>
  {
    public BaseObject(D_BaseObject d_baseObject) : base(d_baseObject) { }

    public static explicit operator BaseObject(D_BaseObject dataBaseObject)
    {  
      return new BaseObject(dataBaseObject);
    }
  }
}
