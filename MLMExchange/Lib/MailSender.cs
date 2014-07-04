using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace MLMExchange.Lib
{
  public class MailSender
  {
    /// <summary>
    /// Конструктор MailSender'а
    /// </summary>
    /// <param name="from">Куда слать</param>
    /// <param name="subject">Тема</param>
    /// <param name="message">Текст письма</param>
    public MailSender(string from, string header, string subject, string message)
    {
      _header = header;
      _fromEmail = from;
      _subject = subject;
      _message = message;
    }
    /// <summary>
    /// Сервер SMTP
    /// </summary>
    private const string smtpHost = "mail.my-crypto.com";
    /// <summary>
    /// Порт SMTP
    /// </summary>
    private const int smtpPort = 587;
    /// <summary>
    /// Имя пользователя от SMTP
    /// </summary>
    private const string smtpUser = "support@my-crypto.com";
    /// <summary>
    /// Пароль от SMTP
    /// </summary>
    private const string smtpPassword = "12345678";
    /// <summary>
    /// Куда слать письмо
    /// </summary>
    public const string supportEmail = "support@my-crypto.com";
    /// <summary>
    /// Адрес отправителя
    /// </summary>
    private readonly string _fromEmail;
    /// <summary>
    /// Отображаемое имя
    /// </summary>
    private readonly string _header;
    /// <summary>
    /// Название темы
    /// </summary>
    private readonly string _subject;
    /// <summary>
    /// Сообщение
    /// </summary>
    private readonly string _message;

    public void SendMailMessage()
    {
      SmtpClient smtpClient = new SmtpClient();
      smtpClient.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPassword);
      smtpClient.Port = smtpPort;
      smtpClient.Host = smtpHost;
      
      smtpClient.Send(getMessage());
    }

    private MailMessage getMessage()
    {
      MailMessage mailMessage = new MailMessage();
      mailMessage.To.Add(supportEmail);
      mailMessage.From = new MailAddress(_fromEmail, _header, System.Text.Encoding.UTF8);
      mailMessage.Subject = _subject;
      mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
      mailMessage.Body = _message;
      mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
      mailMessage.IsBodyHtml = true;
      mailMessage.Priority = MailPriority.Normal;

      return mailMessage;
    }
  }
}