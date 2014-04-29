using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Models.Error
{
  /// <summary>
  /// Ошибка системы
  /// </summary>
  public class ApplicationExceptionModel : BaseModel
  {
    public ApplicationExceptionModel(string title = null)
    {
      if (String.IsNullOrEmpty(title))
        ExceptionTitle = MLMExchange.Properties.ResourcesA.ApplicationExceptionOccupied;
    }

    private string _ExceptionMessage;

    public string ExceptionTitle { get; set; }
    public string ExceptionMessage
    {
      get
      {
        if (String.IsNullOrEmpty(_ExceptionMessage))
          _ExceptionMessage = MLMExchange.Properties.ResourcesA.DataEmpty;

        return _ExceptionMessage;
      }

      set { _ExceptionMessage = value; }
    }
  }
}