using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Lib.Logging
{
  /// <summary>
  /// Общий интерфейс логирования
  /// </summary>
  public interface ILogger<T>
  {
     T LogType { get; set; }

    void Info(string message);

    void Warn(string message);

    void Debug(string message);

    void Error(string message);
    void Error(Exception ex);
    void Error(string message, Exception ex);

    void Fatal(string message);
    void Fatal(Exception ex);
  }

  /// <summary>
  /// Расширение логера, NLog
  /// </summary>
  public class NLogLogger : ILogger<NLogLogger.MessageLogType>
  {
    /// <summary>
    /// NLog Logger
    /// </summary>
    private Logger _Logger;

    public enum MessageLogType
    {
      Info,
      Warn,
      Debug,
      ErrorWithMessage,
      ErrorWithException,
      ErrorWithMessageException,
      FatalError,
      FatalErrorWithException
    }

    /// <summary>
    /// Тип лога
    /// </summary>
    public MessageLogType LogType { get; set; }


    public NLogLogger() 
    {
      _Logger = LogManager.GetCurrentClassLogger();
    }

    public void Info(string message)
    {
      _Logger.Info(message);
    }

    public void Warn(string message)
    {
      _Logger.Warn(message);
    }

    public void Debug(string message)
    {
      _Logger.Debug(message);
    }

    public void Error(string message)
    {
      _Logger.Error(message);
    }

    public void Error(Exception ex)
    {
      Error(LogBuilder.BuildMessageLog(ex, MessageLogType.ErrorWithException));
    }

    public void Error(string message, Exception ex)
    {
      _Logger.Error(message, ex);
    }

    public void Fatal(string message)
    {
      _Logger.Fatal(message);
    }

    public void Fatal(Exception ex)
    {
      Fatal(LogBuilder.BuildMessageLog(ex, MessageLogType.FatalErrorWithException));
    }
  }
}
