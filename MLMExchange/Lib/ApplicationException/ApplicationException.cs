using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Lib.Exception
{
  /// <summary>
  /// Ошибка приложения
  /// </summary>
  public class ApplicationException : System.Exception
  {
    public ApplicationException(string message) : base(message) { }
  }
}