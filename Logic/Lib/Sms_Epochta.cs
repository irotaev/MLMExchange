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
  /// Epochta смс. Отправка смс через шлюз epochta
  /// </summary>
  public class Sms_Epochta
  {
    public Sms_Epochta(string phoneNumber, string messageText)
    {
      _PhoneNumber = phoneNumber;
      _MessageText = messageText;
    }

    private const string _AdminUsername = "worldinvestor69@gmail.com";
    private const string _AdminPassword = "c575fa88";
    private readonly string _PhoneNumber;
    private readonly string _MessageText;

    private XmlDocument BuildXML()
    {
      throw new NotImplementedException();

      XmlDocument xDocument = new XmlDocument();

      XmlNode smsNode = xDocument.CreateElement("SMS");

      XmlNode operationsNode = xDocument.CreateElement("operations");
      XmlNode operationNode = xDocument.CreateElement("operation");
      operationNode.InnerText = "SEND";
      operationsNode.AppendChild(operationNode);

      #region Authentification
      XmlNode authentificationNode = xDocument.CreateElement("authentification");

      XmlNode usernameNode = xDocument.CreateElement("username");
      usernameNode.InnerText = _AdminUsername;

      XmlNode passwordNode = xDocument.CreateElement("password");
      passwordNode.InnerText = _AdminPassword;

      authentificationNode.AppendChild(usernameNode);
      authentificationNode.AppendChild(passwordNode);
      #endregion

      #region Message
      XmlNode messageNode = xDocument.CreateElement("message");
      #endregion
    }
  }
}
