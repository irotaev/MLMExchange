using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Proxy;

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

    /// <summary>
    /// Получить настоящий тип объекта.
    /// Т.е. если это не прокси-объект Nhibernate, то это обычный тип объекта, 
    /// если это прокси-объект, то базовый для прокси типа тип объекта
    /// </summary>
    /// <returns>Реальный тип, не прокси-тип, данного объекта данных</returns>
    public Type GetRealType()
    {
      if (LogicObject.IsProxy())
      {
        return LogicObject.GetType().BaseType;
      }


      return LogicObject.GetType();
    }
  }
}
