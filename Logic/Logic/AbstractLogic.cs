using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
  /// <summary>
  /// Интерфейс базового логического proxy-объекта данных
  /// </summary>
  public interface IAbstractBaseLogicObject
  {
    /// <summary>
    /// Получить базовый объект данных для данного логического proxy-объекта
    /// </summary>
    D_BaseObject BaseLogicObject { get; }
  }

  /// <summary>
  /// Базовый класс для логического объекта. Proxy-объекта
  /// </summary>
  /// <typeparam name="TDataObject">Тип объекта данных, соответствующего логическому proxy-объекту</typeparam>
  public abstract class AbstractLogicObject<TDataObject> : IAbstractBaseLogicObject
    where TDataObject : D_BaseObject
  {
    public AbstractLogicObject(TDataObject dataObject)
    {
      if (dataObject == null)
        throw new ArgumentNullException("logicObject");

      _LogicObject = dataObject;
    }

    protected readonly TDataObject _LogicObject;

    public TDataObject LogicObject { get { return _LogicObject; } }

    public D_BaseObject BaseLogicObject
    {
      get
      {
        return _LogicObject;
      }
    }
  }
}
