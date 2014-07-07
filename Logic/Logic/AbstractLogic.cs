using NHibernate;
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
  /// Показывает, что это объект данных.
  /// Введен для архитектурной связи уровня данных и уровня прокси-логики.
  /// </summary>
  public interface IEntityObject { }

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
  /// Реализует события NHibernate
  /// </summary>
  public interface INhibernateEvent
  {
    /// <summary>
    /// Вызывается перед внесением изменений, касающихся данного объекта, в базу
    /// <param name="event">Nhibernate pre insert event</param>
    /// </summary>
    void OnPreInsert(NHibernate.Event.PreInsertEvent @event);

    /// <summary>
    /// Вызывается перед апдейтом данных, касающихся данного объекта, в базе
    /// <param name="event">NHibernate pre update event</param>
    /// </summary>
    void OnPreUpdate(NHibernate.Event.PreUpdateEvent @event);
  }

  /// <summary>
  /// Базовый класс для логического объекта. Proxy-объекта
  /// </summary>
  /// <typeparam name="TDataObject">Тип объекта данных, соответствующего логическому proxy-объекту</typeparam>
  public abstract class AbstractLogicObject<TDataObject> : IAbstractBaseLogicObject, INhibernateEvent, IEntityObject
    where TDataObject : D_BaseObject
  {
    public AbstractLogicObject(TDataObject dataObject)
    {
      if (dataObject == null)
        throw new ArgumentNullException("logicObject");

      _LogicObject = dataObject;
      _NhibernateSession = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;
    }

    protected readonly ISession _NhibernateSession;
    protected readonly TDataObject _LogicObject;

    public TDataObject LogicObject { get { return _LogicObject; } }

    public D_BaseObject BaseLogicObject
    {
      get
      {
        return _LogicObject;
      }
    }

    public virtual void OnPreInsert(NHibernate.Event.PreInsertEvent @event) { }

    public virtual void OnPreUpdate(NHibernate.Event.PreUpdateEvent @event) { }
  }
}
