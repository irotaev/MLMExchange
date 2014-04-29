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
    where TObject : BaseObject
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
  /// Интерфейс модели, связанной с объектом данных
  /// </summary>
  public interface IDataModel
  {
    long? Id { get; set; }
  }

  public abstract class BaseModel
  {
  }

  public abstract class AbstractDataModel : BaseModel, IDataModel, IDataBinding<BaseObject, AbstractDataModel>
  {
    [HiddenInput]
    public long? Id { get; set; }

    public virtual AbstractDataModel Bind(BaseObject @object)
    {
      Id = @object.Id;

      return this;
    }

    public virtual BaseObject UnBind(BaseObject @object = null)
    {
      if (@object == null)
        throw new ArgumentOutOfRangeException("object");

      return @object;
    }
  }

  public abstract class AbstractDataModel<TObject, TModel> : AbstractDataModel, IDataBinding<TObject, TModel>
    where TObject : BaseObject, new()
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

  #region Exceptions
  public class ModelInvalidException : ApplicationException
  {
    public ModelInvalidException() : base() { }
    public ModelInvalidException(string message) : base(message) { }
    public ModelInvalidException(string message, Exception innerException) : base(message, innerException) { }
  }
  #endregion
}