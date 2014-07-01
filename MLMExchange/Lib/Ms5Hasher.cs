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
    /// Конвертировать строку в хэш. Используется петтерн "динамической соли". 
    /// 
    /// Если строка имеет больше 4 символов, то динамическая соль будет генерироваться на основе них,
    /// если не имеет, то динамическая соль генерируется по первому символу
    /// </summary>
    /// <param name="str">Конвертируемая строка</param>
    /// <returns>Хэш</returns>
    public static string ConvertStringToHash(string str)
    {
      if (String.IsNullOrWhiteSpace(str))
        return null;

      MD5 md5Hasher = MD5.Create();

      #region Salt
      string salt = String.Empty;
      {
        byte[] saltAsByte = null;

        if (str.Length >= 6)
        {
          saltAsByte = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str.Substring(1, 5)));
        }
        else if (str.Length >= 4)
        {
          saltAsByte = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str.Substring(0, 3)));
        }
        else
        {
          saltAsByte = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str.Substring(0, 0)));
        }

        salt = System.Text.Encoding.UTF8.GetString(saltAsByte);
      }
      #endregion

      byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str + salt));

      StringBuilder sBuilder = new StringBuilder();

      for (int i = 0; i < data.Length; i++)
      {
        sBuilder.Append(data[i].ToString("x2"));
      }

      return sBuilder.ToString();
    }
  }
}
