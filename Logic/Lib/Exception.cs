using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Lib
{
  /// <summary>
  /// Исключение
  /// </summary>
  public static class ExceptionExtension
  {
    /// <summary>
    /// Получить лог по всему дереву ошибок, включая внутрении
    /// </summary>
    /// <param name="exception">Ошибка</param>
    /// <param name="splitSymbol">Символ-разделитель между ошибками</param>
    /// <returns>Лог ошибки</returns>
    public static String GetAllExceptionTreeLog(this Exception exception, string splitSymbol)
    {
      string log = String.Empty;

      if (exception == null)
        return log;

      StringBuilder sb = new StringBuilder(log);

      Action<Exception> getExceptionLog = null;
      getExceptionLog = (Exception ex) =>
        {
          if (sb.Length != 0)
            sb.Append(splitSymbol);

          sb.Append(ex.Message);

          if (ex.InnerException != null)
            getExceptionLog(ex.InnerException);  
        };

      getExceptionLog(exception);

      log = sb.ToString();

      return log;
    }

    /// <summary>
    /// Получить лог по всему дереву ошибок, включая внутрении
    /// </summary>
    /// <param name="exception">Ошибка</param>
    /// <returns>Лог ошибки</returns>
    public static String GetAllExceptionTreeLog(this Exception exception)
    {
      return exception.GetAllExceptionTreeLog(Environment.NewLine);
    }
  }
}
