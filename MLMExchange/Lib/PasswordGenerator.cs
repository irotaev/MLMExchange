using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Lib
{
  /// <summary>
  /// Кастомный генератор паролей
  /// </summary>
  /// Доработать
  public class PasswordGenerator
  {
    /// <summary>
    /// Дефолтная длинна пароля
    /// </summary>
    private static int DefaultPasswordLenght = 8;

    /// <summary>
    /// Символы в пароле
    /// </summary>
    public static string PasswordChars = "abcdefghjkmnpqrstwxyzABCDEFGHJKMNPQRSTWXYZ";
    public static string PasswordCharsNumeric = "0123456789";
    public static string PasswordChardsWithNumberic = PasswordChars + PasswordCharsNumeric;

    public static string Gengreate()
    {
      return Gengreate(DefaultPasswordLenght, PasswordChardsWithNumberic);
    }

    public static string Gengreate(int passwordLenght)
    {
      return Gengreate(passwordLenght, PasswordChardsWithNumberic);
    }

    public static string Gengreate(string nonStandartCharacters)
    {
      return Gengreate(DefaultPasswordLenght, nonStandartCharacters);
    }

    public static string Gengreate(int passwordLength, string passwordChars)
    {
      return GengreatePassword(passwordLength,
                              passwordChars);
    }

    private static string GengreatePassword(int passwordLenght, string passwordCharacters)
    {
      if (passwordLenght < 0)
        throw new ArgumentOutOfRangeException("passwordLenght");
      
      if (string.IsNullOrEmpty(passwordCharacters))
        throw new ArgumentOutOfRangeException("passwordCharacters");
                
      var password = new char[passwordLenght];
      var random = GetRandom();

      for (int i = 0; i < passwordLenght; i++)
      {
        password[i] = passwordCharacters[random.Next(passwordCharacters.Length)];
      }
    
      return password.ToString();
    }

    private static Random GetRandom()
    {
      //TODO: Переделать, можно предугадать seed
      return new Random((int)DateTime.Now.Ticks);
    }
  }
}