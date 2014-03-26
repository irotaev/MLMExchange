using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Lib
{
  abstract public class BaseFile
  {
    public static string ServerAbsolutePath
    {
      get
      {
        return HttpContext.Current.Server.MapPath("~");
      }
    }
  }
}