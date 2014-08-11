using Logic;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

namespace MLMExchange.Models
{
  #region Интерфейсы
  /// <summary>
  /// Интерфейс абстрактной модели.
  /// </summary>
  public interface IAbstractModel
  {

  }

  /// <summary>
  /// Модель, позволяющая использовать "лейзи-лоадинг".
  /// Лайзи-лоадинг позволяет загружать свойства в момент досптупа к ним, 
  /// тем самым выигрывается производительность там, где при обращении к свойству происходит 
  /// ресурсоемкая операция, например, загрузка большого количества объектов из базы и их 
  /// преобразования.
  /// </summary>
  public interface ILazyLoadModel
  {
    /// <summary>
    /// Выключить ли лайзи-лоадинг для данной модели.
    /// По-умолчанию лайзи-лоадинг включен.
    /// </summary>
    bool IsLazyLoadingDisable { get; set; }
  }

  /// <summary>
  /// Интерфейс частной валидации
  /// </summary>
  public interface ICustomValidation
  {
    /// <summary>
    /// Частная валидация. 
    /// Одно из частых применений - вызывать перед вызовом UnBind модели.
    /// </summary>
    /// <param name="modelState">Состоянии текущей модели. Туда заносятся ошибки</param>
    void CustomValidation(System.Web.Mvc.ModelStateDictionary modelState, MLMExchange.Models.AbstractModel._CustomValidationSettings settings = null);
  }

  /// <summary>
  /// Интерфейс биндинга к данным. 
  /// Однонаправленый, только получение данных из объекта.
  /// Используется для получения данных из объекта в модель.
  /// </summary>
  /// <typeparam name="TObject">Тип объекта биндинга</typeparam>
  /// <typeparam name="TModel">Тип модели данных</typeparam>
  public interface IDataGetter<TObject, out TModel>
    where TObject : D_BaseObject
    where TModel : AbstractModel
  {
    TModel Bind(TObject @object);
  }

  /// <summary>
  /// Интерфейс биндинга к данным. 
  /// Однонаправленый, только получение данных из объекта.
  /// Используется для получения данных из объекта в модель.
  /// </summary>
  /// <typeparam name="TLogicObject">Тип объекта биндинга. Объект прокси-логики</typeparam>
  /// <typeparam name="TModel">Тип модели данных</typeparam>
  public interface ILogicDataGetter<TLogicObject, out TModel>
    where TLogicObject : class, IAbstractBaseLogicObject
    where TModel : AbstractModel
  {
    TModel Bind(TLogicObject @object);
  }

  /// <summary>
  /// Интерфейс биндинга к данным. 
  /// Двуноправленный, т.е. получение данных из объекта, и сохранение данных в объект.
  /// Используется для биндинга объекта данных в модель.
  /// <typeparam name="TObject">Объект данных</typeparam>
  /// <typeparam name="TModel">Тип модели данных</typeparam>
  /// </summary>
  public interface IDataBinding<TObject, out TModel>
    where TObject : D_BaseObject
    where TModel : class, IAbstractModel
  {
    /// <summary>
    /// Биндинг в web-модель. Прокси-биндинг
    /// </summary>
    /// <param name="object">Объект данных</param>
    /// <returns>Web-модель</returns>
    TModel Bind(TObject @object);
    /// <summary>
    /// Аниндинг в объект данных. Прокси-биндинг
    /// </summary>
    /// <param name="object">Уже существующий объект для анбиндинга, в него буду добавлены свойства</param>
    /// <returns>Объект данных</returns>
    TObject UnBind(TObject @object = null);
  }

  /// <summary>
  /// Интерфейс биндинга к данным
  /// Двуноправленный, т.е. получение данных из объекта, и сохранение данных в объект.
  /// Используется для биндинга объекта данных в модель.
  /// <typeparam name="TLogicObject">Тип объекта логики данных</typeparam>
  /// <typeparam name="TModel">Тип модели данных</typeparam>
  /// </summary>
  public interface ILogicDataBinding<TLogicObject, out TModel>
    where TLogicObject : class, IAbstractBaseLogicObject
    where TModel : AbstractModel
  {
    /// <summary>
    /// Биндинг в web-модель. Прокси-биндинг
    /// </summary>
    /// <param name="object">Объект данных</param>
    /// <returns>Web-модель</returns>
    TModel Bind(TLogicObject @object);
    /// <summary>
    /// Аниндинг в объект данных. Прокси-биндинг
    /// </summary>
    /// <param name="object">Уже существующий объект для анбиндинга, в него буду добавлены свойства</param>
    /// <returns>Объект данных</returns>
    TLogicObject UnBind(TLogicObject @object = null);
  }

  /// <summary>
  /// Интерфейс модели, связанной с объектом данных
  /// </summary>
  public interface IDataModel
  {
    long? Id { get; set; }
  }
  #endregion

  /// <summary>
  /// Базовая модель.
  /// </summary>
  public abstract class AbstractModel : ILazyLoadModel, IAbstractModel, ICustomValidation
  {
    protected readonly ISession _NhibernateSession = Logic.Lib.ApplicationUnityContainer.UnityContainer.Resolve<INHibernateManager>().Session;

    #region Text resources    
    protected readonly Dictionary<string, object> _TextResources = new Dictionary<string, object>();

    /// <summary>
    /// Текстовые ресурсы, поставляемые с данной моделью.
    /// Используются при отрисовки модели на клиенте, когда используется технология REST/FULL REST.
    /// </summary>
    public Dictionary<string, object> TextResources
    {
      get
      {
        return _TextResources;
      }
    }
    #endregion

    public bool IsLazyLoadingDisable { get; set; }

    #region Custom validation
    public virtual void CustomValidation(ModelStateDictionary modelState, _CustomValidationSettings settings = null) { }

    public class _CustomValidationSettings { }
    #endregion
  }

  /// <summary>
  /// Базовая модель. 
  /// Используется для биндинга сущностей с уровня данных.
  /// </summary>
  public abstract class AbstractDataModel : AbstractModel, IDataModel, IDataBinding<D_BaseObject, AbstractDataModel>
  {
    [NonSerialized]
    protected IAbstractBaseLogicObject _Object;

    #region Data lazy load
    /// <summary>
    /// Сюда заносятся свойства, которые необходимо подгружать посредством механизма lazy loading.
    /// Как правило свойства заносятся в статическом конструкторе.
    /// В формате:
    /// key - имя свойство, которое должно быть загружено механизмом lazy loading,
    /// value - это флаг, который показывает нужно ли свойство биндить из объекта, либо такой необходимости нет.
    /// 
    /// Это значит что, при вызове метода Bind, всем свойства, занесенным в данный словарь, ставится флаг true.
    /// Значит при вызове геттера у свойства значение надо взять из объекта данных. Далее возможна такая ситуация, 
    /// что вызывается сеттер у свойства и мы можем руками поменять флаг в словаре на false, это значит что при следующем обращении к 
    /// геттеру свойства значение надо брать уже из другого места, например, из отдельного поля, где было сохранено новое значение,
    /// заданное руками через сетттер свойства.
    /// </summary>
    protected readonly Dictionary<string, bool> _LazyLoadProperties = new Dictionary<string, bool>();
    #endregion

    [HiddenInput(DisplayValue = false)]
    public long? Id { get; set; }
    /// <summary>
    /// Дата создания объекта
    /// </summary>
    [HiddenInput(DisplayValue = false)]
    public DateTime CreationDateTime { get; set; }

    #region General Bind/Unbind
    private void GeneralBind()
    {
      foreach(var key in _LazyLoadProperties.Keys.ToList())
      {
        _LazyLoadProperties[key] = true;
      }
    }

    private void GeneralUnbind()
    {

    }
    #endregion

    #region For data objects
    public virtual AbstractDataModel Bind(D_BaseObject @object)
    {
      GeneralBind();

      Id = @object.Id;
      CreationDateTime = @object.CreationDateTime;

      return this;
    }

    public virtual D_BaseObject UnBind(D_BaseObject @object = null)
    {
      GeneralUnbind();

      if (@object == null)
        throw new ArgumentOutOfRangeException("object");

      return @object;
    }
    #endregion

    #region For logic objects
    public virtual AbstractDataModel Bind(IAbstractBaseLogicObject @object)
    {
      if (@object == null)
        throw new ArgumentNullException("@object");

      _Object = @object;

      Id = @object.BaseLogicObject.Id;
      CreationDateTime = @object.BaseLogicObject.CreationDateTime;

      return this;
    }

    public virtual IAbstractBaseLogicObject UnBind(IAbstractBaseLogicObject @object = null)
    {
      if (@object == null)
        throw new ArgumentOutOfRangeException("object");

      return @object;
    }
    #endregion
  }

  /// <summary>
  /// Базовая модель данных. Для объектов данных
  /// </summary>
  /// <typeparam name="TObject">Тип объекта данных</typeparam>
  /// <typeparam name="TModel">Тип модели</typeparam>
  public abstract class AbstractDataModel<TObject, TModel> : AbstractDataModel, IDataBinding<TObject, TModel>
    where TObject : D_BaseObject
    where TModel : class, IAbstractModel
  {
    [NonSerialized]
    new protected TObject _Object;

    public virtual TModel Bind(TObject @object)
    {
      base.Bind(@object);

      _Object = @object;

      return this as TModel;
    }

    public virtual TObject UnBind(TObject @object = null)
    {
      return (TObject)base.UnBind(@object);
    }
  }

  /// <summary>
  /// Базовая модель данных. Для логических объектов
  /// </summary>
  /// <typeparam name="TLogicObject">Тип логического объекта данных. Proxy-объекта</typeparam>
  /// <typeparam name="TDataObject">Тип объекта данных</typeparam>
  /// <typeparam name="TModel">Тип модели</typeparam>
  public abstract class AbstractDataModel<TLogicObject, TDataObject, TModel> : AbstractDataModel, ILogicDataBinding<TLogicObject, TModel>
    where TLogicObject : AbstractLogicObject<TDataObject>
    where TDataObject : D_BaseObject, new()
    where TModel : AbstractModel
  {
    new protected TLogicObject _Object;

    public virtual TModel Bind(TLogicObject @object)
    {
      if (@object == null)
        throw new ArgumentNullException("@object");

      _Object = @object;

      base.Bind(@object);

      return this as TModel;
    }

    public virtual TLogicObject UnBind(TLogicObject @object = null)
    {
      throw new NotImplementedException();
    }
  }

  #region Exceptions
  /// <summary>
  /// Базовая ошибка модели
  /// </summary>
  public abstract class ModelException : ApplicationException 
  {
    public ModelException() : base() { }
    public ModelException(string message) : base(message) { }
    public ModelException(string message, Exception innerException) : base(message, innerException) { }
  }

  public class ModelInvalidException : ModelException
  {
    public ModelInvalidException() : base() { }
    public ModelInvalidException(string message) : base(message) { }
    public ModelInvalidException(string message, Exception innerException) : base(message, innerException) { }
  }

  /// <summary>
  /// Не вызван прямой биндинг для конкретного объекта данных. 
  /// В этом случаи нельзя получить свойства, относящиеся к свойствам конкретного объекта данных
  /// <typeparam name="TDataObject">Тип объекта данных</typeparam>
  /// </summary>
  public class BindNotCallException<TDataObject> : ModelException
    where TDataObject : IEntityObject
  {
    public BindNotCallException() : base() 
    {
      _Message = String.Format(MLMExchange.Properties.ResourcesA.Model__Exception_BindNotCall, typeof(TDataObject).Name);
    }

    private readonly string _Message;

    public override string Message
    {
      get
      {
        return _Message;
      }
    }
  }

  #endregion
}