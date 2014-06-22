using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Models
{
  /// <summary>
  /// Интерфейс биндинга к данным
  /// <typeparam name="TObject">Объект данных</typeparam>
  /// </summary>
  public interface IDataBinding<TObject, out TModel>
    where TObject : D_BaseObject
    where TModel : BaseModel
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
  /// <typeparam name="TLogicObject">Тип объекта логики данных</typeparam>
  /// </summary>
  public interface ILogicDataBinding<TLogicObject, out TModel>
    where TLogicObject : class, IAbstractBaseLogicObject
    where TModel : BaseModel
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

  public abstract class BaseModel
  {
  }

  public abstract class AbstractDataModel : BaseModel, IDataModel, IDataBinding<D_BaseObject, AbstractDataModel>
  {
    [HiddenInput(DisplayValue = false)]
    public long? Id { get; set; }
    /// <summary>
    /// Дата создания объекта
    /// </summary>
    [HiddenInput(DisplayValue = false)]
    public DateTime CreationDateTime { get; set; }


    #region For data objects
    public virtual AbstractDataModel Bind(D_BaseObject @object)
    {
      Id = @object.Id;
      CreationDateTime = @object.CreationDateTime;

      return this;
    }

    public virtual D_BaseObject UnBind(D_BaseObject @object = null)
    {
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
    where TObject : D_BaseObject, new()
    where TModel : BaseModel
  {
    public virtual TModel Bind(TObject @object)
    {
      base.Bind(@object);

      return this as TModel;
    }

    public virtual TObject UnBind(TObject @object = null)
    {
      if (@object == null)
        @object = new TObject();

      return (TObject)base.UnBind(@object);
    }
  }

  #region AbstractDataModel with operator casting
  /// <summary>
  /// Базовая модель данных. Для логических объектов
  /// </summary>
  /// <typeparam name="TLogicObject">Тип логического объекта данных. Proxy-объекта</typeparam>
  /// <typeparam name="TDataObject">Тип объекта данных</typeparam>
  /// <typeparam name="TModel">Тип модели</typeparam>
  public abstract class AbstractDataModel<TLogicObject, TDataObject, TModel> : AbstractDataModel, ILogicDataBinding<TLogicObject, TModel>
    where TLogicObject : AbstractLogicObject<TDataObject>
    where TDataObject : D_BaseObject, new()
    where TModel : BaseModel
  {
    public virtual TModel Bind(TLogicObject @object)
    {
      if (@object == null)
        throw new ArgumentNullException("@object");

      base.Bind(@object);

      return this as TModel;
    }

    public virtual TLogicObject UnBind(TLogicObject @object = null)
    {
      throw new NotImplementedException();
    }
  }
  #endregion

  #region Exceptions
  public class ModelInvalidException : ApplicationException
  {
    public ModelInvalidException() : base() { }
    public ModelInvalidException(string message) : base(message) { }
    public ModelInvalidException(string message, Exception innerException) : base(message, innerException) { }
  }
  #endregion
}