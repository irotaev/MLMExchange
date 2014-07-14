using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Lib
{
  /// <summary>
  /// Ошибка приложения
  /// </summary>
  public class ApplicationException : System.Exception
  {
    public ApplicationException() : base() { }
    public ApplicationException(string message) : base(message) { }
    public ApplicationException(string message, System.Exception ex) : base(message, ex) { }
  }

  /// <summary>
  /// Ошибка приложения.
  /// Видна пользователю.
  /// </summary>
  public class UserVisibleException : ApplicationException
  {
    public UserVisibleException() : base() { }
    public UserVisibleException(string message) : base(message) { }
    public UserVisibleException(string message, System.Exception ex) : base(message, ex) { }
  }

  /// <summary>
  /// Ошибка приложения. Аргумент не задан.
  /// Видна пользователю.
  /// </summary>
  public class UserVisible__ArgumentNullException : UserVisibleException
  {
    public UserVisible__ArgumentNullException(string argumentName)
      : base()
    {
      throw new UserVisibleException(String.Format(Logic.Properties.GeneralResources.Exception_ArgumentNull, argumentName));
    }
  }

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
