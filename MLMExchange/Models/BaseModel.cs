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
  public interface IDataBinding<TObject> 
    where TObject : BaseObject
  {
    /// <summary>
    /// Биндинг в web-модель. Прокси-биндинг
    /// </summary>
    /// <param name="object">Объект данных</param>
    /// <returns>Web-модель</returns>
    void Bind(TObject @object);
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
    long Id { get; set; }
  }

  public abstract class BaseModel
  {
  }

  public abstract class AbstractDataModel : BaseModel, IDataModel
  {
    [HiddenInput]
    public long? Id { get; set; }
  }
}