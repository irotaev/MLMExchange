using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Logic.Lib
{
  /// <summary>
  /// Обертка библиотеки с сайта http://ssms.su
  /// </summary>
  public class Ssms
  {
    private const string _ServerAdress = "api2.ssms.su";
    private const string _Email = "mycrypto2014@gmail.com";
    private const string _Password = "e8j3Za";
    private const string _DefaultSenderName = "My-Crypto";

    private readonly Dictionary<string, string> _QuesryParams = new Dictionary<string, string>();

    private void AddQueryParam(string key, string value)
    {
      if (_QuesryParams.Keys.Contains(key))
        throw new Logic.Lib.ApplicationException("Данный ключ уже добавлен в коллекцию");

      _QuesryParams.Add(key, value);
    }

    private string BuildRequestUrl(BuildUrlType buildUrlType)
    {
      string method;
      if (_QuesryParams.TryGetValue("method", out method))
      {
        _QuesryParams.Remove("method");
      }
      else
      {
        throw new ApplicationException("method doesnt exist");
      }

      if (!_QuesryParams.ContainsKey("sender_name"))
        _QuesryParams.Add("sender_name", _DefaultSenderName);

      string otherParametrs = String.Join("&", _QuesryParams.Select(x => { return x.Key + "=" + System.Web.HttpUtility.UrlEncode(x.Value); }).ToArray());

      string url = null;

      if (buildUrlType == BuildUrlType.Data)
      {
        url = String.Format(@"method={0}&email={1}&password={2}&{3}", method, _Email, _Password, otherParametrs);
      }
      else if (buildUrlType == BuildUrlType.Full)
      {
        url = String.Format(@"http://{0}/?method={1}&email={2}&password={3}&{4}", _ServerAdress, method, _Email, _Password, otherParametrs);
      }

      return url;
    }

    private _ServerResponse ParseServerResponse(string response)
    {
      XmlDocument xml = new XmlDocument();
      xml.LoadXml(response);

      _ServerResponse parsedResponse;

      try
      {
        var msgNode = xml.SelectSingleNode("/response/msg");

        var err_codeNode = msgNode.SelectSingleNode("err_code");
        var textNode = msgNode.SelectSingleNode("text");
        var typeNode = msgNode.SelectSingleNode("type");
        //var dataNode = msgNode.SelectSingleNode("data");

        parsedResponse = new _ServerResponse(err_codeNode.InnerText, textNode.InnerText, typeNode.InnerText);
      }
      catch(Exception ex)
      {
        throw new _ServerResponseParseException(ex);
      }

      return parsedResponse;
    }

    /// <summary>
    /// Попробовать отправить сообщение
    /// </summary>
    /// <param name="message">Текст сообщения</param>
    /// <param name="phoneNumber">Номер телефона в международном формате (79854667722)</param>
    /// <returns>Ответ сервера</returns>
    public _ServerResponse SendMessage(string message, string phoneNumber)
    {
      AddQueryParam("method", "push_msg");
      AddQueryParam("text", message);
      AddQueryParam("phone", phoneNumber);

      _ServerResponse serverResponse;

      using (WebClient webClient = new WebClient())
      {
        string unparsedResponse;

        try
        {
          unparsedResponse = webClient.DownloadString(BuildRequestUrl(BuildUrlType.Full));
        }
        catch(Exception ex)
        {
          throw new _RemoteServerException(ex.Message);
        }

        serverResponse = ParseServerResponse(unparsedResponse);

        if (serverResponse.ErrorCode != "0")
          throw new _RemoteServerException(serverResponse.Text);
      }

      return serverResponse;
    }

    /// <summary>
    /// Сгенерировать уникальный код для смс
    /// </summary>
    /// <returns>Код</returns>
    public static string GenerateSmsCode()
    {
      string code = System.IO.Path.GetRandomFileName().Replace(".", "");

      if (code.Length >= 6)
        code = code.Substring(0, 6);

      return code;
    }

    private enum BuildUrlType
    {
      Full = 0,
      Data = 1
    }

    /// <summary>
    /// Ответ от сервера
    /// </summary>
    public struct _ServerResponse
    {
      public _ServerResponse(string errorCode, string text, string type)
      {
        ErrorCode = errorCode;
        Text = text;
        Type = type;
      }

      public readonly string ErrorCode;
      public readonly string Text;
      public readonly string Type;
      //public readonly string Data;
    }

    #region Exceptions
    public abstract class _SsmsException : ApplicationException
    {
      public _SsmsException() : base() { }
      public _SsmsException(string message) : base(message) { }
      public _SsmsException(string message, System.Exception ex) : base(message, ex) { }
    }

    public class _RemoteServerException : _SsmsException
    {
      public _RemoteServerException() : base() { }
      public _RemoteServerException(string message) : base(message) { }
      public _RemoteServerException(string message, System.Exception ex) : base(message, ex) { }
    }

    public class _ServerResponseParseException : _SsmsException
    {
      public _ServerResponseParseException(Exception ex)
      {
        _Message = String.Format(Logic.Properties.PrivateResources.Ssms__Exception_ServerResponseParse, ex.Message);
      }

      private readonly string _Message;

      public override string Message
      {
        get
        {
          return _Message;
        }
      }
    }
    #endregion
  }
}
