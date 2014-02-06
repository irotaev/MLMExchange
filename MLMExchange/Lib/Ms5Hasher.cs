using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Lib
{
  /// <summary>
  /// Хэш
  /// </summary>
  public class Md5Hasher
  {
    /// <summary>
    /// Конвертировать строку в хэш
    /// </summary>
    /// <param name="str">Конвертируемая строка</param>
    /// <returns>Хэш</returns>
    public static string ConvertStringToHash(string str)
    {
      if (String.IsNullOrEmpty(str))
        return null;

      MD5 md5Hasher = MD5.Create();

      byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));

      StringBuilder sBuilder = new StringBuilder();

      for (int i = 0; i < data.Length; i++)
      {
        sBuilder.Append(data[i].ToString("x2"));
      }

      return sBuilder.ToString();
    }
  }
}
