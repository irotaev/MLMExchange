using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Logic.Lib
{
  /// <summary>
  /// Epochta смс. Отправка смс через шлюз epochta
  /// </summary>
  public class Sms_Epochta
  {
    public Sms_Epochta(string phoneNumber)
    {
      _PhoneNumber = phoneNumber;
    }

    private const string _AdminUsername = "worldinvestor69@gmail.com";
    private const string _AdminPassword = "c575fa88";
    private const string _Sender = "My-crypto";
    private readonly string _PhoneNumber;

    private XmlDocument BuildSendXML(string message)
    {
      XmlDocument xDocument = new XmlDocument();
      xDocument.CreateXmlDeclaration("1.0", "UTF-8", "yes");

      XmlNode smsNode = xDocument.CreateElement("SMS");

      xDocument.AppendChild(smsNode);

      XmlNode operationsNode = xDocument.CreateElement("operations");
      XmlNode operationNode = xDocument.CreateElement("operation");
      operationNode.InnerText = "SEND";
      operationsNode.AppendChild(operationNode);

      smsNode.AppendChild(operationsNode);

      #region Authentification
      XmlNode authentificationNode = xDocument.CreateElement("authentification");

      XmlNode usernameNode = xDocument.CreateElement("username");
      usernameNode.InnerText = _AdminUsername;

      XmlNode passwordNode = xDocument.CreateElement("password");
      passwordNode.InnerText = _AdminPassword;

      authentificationNode.AppendChild(usernameNode);
      authentificationNode.AppendChild(passwordNode);

      smsNode.AppendChild(authentificationNode);
      #endregion

      #region Message
      XmlNode messageNode = xDocument.CreateElement("message");

      XmlNode senderNode = xDocument.CreateElement("sender");
      senderNode.InnerText = _Sender;

      XmlNode textNode = xDocument.CreateElement("text");
      textNode.InnerText = message; //String.Format(@"<![CDATA[{0}]]>", message);

      messageNode.AppendChild(senderNode);
      messageNode.AppendChild(textNode);

      smsNode.AppendChild(messageNode);
      #endregion

      #region Numbers
      XmlNode numbersNode = xDocument.CreateElement("numbers");
      XmlNode numberNode = xDocument.CreateElement("number");
      numberNode.InnerText = _PhoneNumber;

      numbersNode.AppendChild(numberNode);
      smsNode.AppendChild(numbersNode);
      #endregion

      return xDocument;
    }

    private string BuildSendString(string message)
    {
      string result = null;
      XmlDocument xDoc = BuildSendXML(message);

      using (_StringWriterUTF8 strWriter = new _StringWriterUTF8())
      {
        xDoc.Save(strWriter);
        result = strWriter.ToString();
      }

      return result;
    }

    /// <summary>
    /// Послать сообщение
    /// <param name="message">Сообщение для отсылки</param>
    /// </summary>
    public void SendMessage(string message)
    {
      HttpWebRequest request = WebRequest.Create("http://atompark.com/members/sms/xml.php") as HttpWebRequest;
      request.Method = "Post";
      request.ContentType = "application/x-www-form-urlencoded";

      byte[] data = new UTF8Encoding().GetBytes(BuildSendString(message));
      request.ContentLength = data.Length;
      System.IO.Stream dataStream = request.GetRequestStream();
      dataStream.Write(data, 0, data.Length);

      using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
      {
        if (response.StatusCode != HttpStatusCode.OK)
          throw new BaseSms_EpochtaException(String.Format(Logic.Properties.PrivateResources.Sms_Epochta__Exception_ServerREquestFailed, response.StatusCode, response.StatusDescription));

        StreamReader reader = new StreamReader(response.GetResponseStream());

        //string resp = reader.ReadToEnd();
      }
    }

    private class _StringWriterUTF8 : StringWriter
    {
      public override Encoding Encoding
      {
        get { return Encoding.UTF8; }
      }
    }

    #region Generate sms code
    public const ushort SmsCodeLength = 6;

    /// <summary>
    /// Сгенерировать код
    /// </summary>
    /// <returns>Код. Примерно 6 символов</returns>
    public static string GenerateSmsCode()
    {
      return Guid.NewGuid().ToString("n").Substring(0, SmsCodeLength);
    }
    #endregion
  }

  #region Exceptions
  public class BaseSms_EpochtaException : Logic.Lib.ApplicationException
  {
    public BaseSms_EpochtaException() : base() { }
    public BaseSms_EpochtaException(string message) : base(message) { }
    public BaseSms_EpochtaException(string message, System.Exception ex) : base(message, ex) { }
  }

  /// <summary>
  /// Ошибка, которуювернул сервер
  /// </summary>
  public class Sms_EpochtaResponseException : BaseSms_EpochtaException
  {
    public Sms_EpochtaResponseException(string code, string description)
    {
      _Message = String.Format(Logic.Properties.PrivateResources.Sms_Epochta__Exception_ServerResponseFailed, code, description);
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
