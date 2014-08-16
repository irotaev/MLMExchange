using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Lib.Logging;

namespace Logic.Lib.Logging
{
  class LogBuilder
  {
    public static string BuildMessageLog(Exception ex, NLogLogger.MessageLogType logType)
    {
      string errorMessage = String.Empty;

      errorMessage = Environment.NewLine + "["+ logType.ToString() +"] Path: " + System.Web.HttpContext.Current.Request.Path;
      errorMessage += Environment.NewLine + "URL: " + System.Web.HttpContext.Current.Request.Url;
      errorMessage += Environment.NewLine + "Message: " + ex.Message;
      errorMessage += Environment.NewLine + "Source: " + ex.Source;
      errorMessage += Environment.NewLine + "Stack Trace: " + ex.StackTrace;
      errorMessage += Environment.NewLine + "Requests IP: " + System.Web.HttpContext.Current.Request.UserHostAddress;
      errorMessage += Environment.NewLine + "UserAgent: " + System.Web.HttpContext.Current.Request.UserAgent;

      return errorMessage;
    }
  }
}
