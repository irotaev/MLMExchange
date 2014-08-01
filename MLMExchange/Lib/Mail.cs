using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace MLMExchange.Lib
{
  public class Mail
  {
    /// <summary>
    /// Конструктор MailSender'а
    /// </summary>
    /// <param name="from">Куда слать</param>
    /// <param name="subject">Тема</param>
    /// <param name="message">Текст письма</param>
    public Mail(string from, string header, string subject, string message)
    {
      _header = header;
      _fromEmail = from;
      _subject = subject;
      _message = message;
      State = MailState.NotSend;
    }

    /// <summary>
    /// Перегрузка MailSendera,
    /// возможность слать на кастомный адресс
    /// </summary>
    /// <param name="from">Откуда</param>
    /// <param name="header"></param>
    /// <param name="subject">Тема</param>
    /// <param name="message">Текст письма</param>
    /// <param name="to">Куда слать</param>
    public Mail(string from, string header, string subject, string message, string to)
    {
      _header = header;
      _fromEmail = from;
      _subject = subject;
      _message = message;
      _to = to;
      State = MailState.NotSend;
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
    /// <summary>
    /// Куда слать (кастомно)
    /// </summary>
    private readonly string _to;
    /// <summary>
    /// Состояние сообщения
    /// </summary>
    public MailState State;

    public enum MailState : int
    { 
      NotSend = 0,
      Sending = 1,
      Sended = 2,
      Error = 3,
      Canceled = 4
    }

    private string _userToken { get { return Guid.NewGuid().ToString(); } }

    /// <summary>
    /// Отправить сообщение
    /// использую SMTP протокол
    /// </summary>
    public void SendMailMessage()
    {
      SmtpClient smtpClient = new SmtpClient();
      smtpClient.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPassword);
      smtpClient.Port = smtpPort;
      smtpClient.Host = smtpHost;

      State = MailState.Sending;
      smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
      smtpClient.Send(getMessage());

      if (State == MailState.Error)
      {
        smtpClient.SendAsyncCancel();
      }
      smtpClient.Dispose();
    }

    /// <summary>
    /// Преобразую в сообщение
    /// </summary>
    /// <returns>собранное сообщение, готовое для отправки</returns>
    private MailMessage getMessage()
    {
      MailMessage mailMessage = new MailMessage();

      if (String.IsNullOrEmpty(_to))
      {
        mailMessage.To.Add(supportEmail);
      }
      else
      {
        mailMessage.To.Add(_to);
      }

      mailMessage.From = new MailAddress(_fromEmail, _header, System.Text.Encoding.UTF8);
      mailMessage.Subject = _subject;
      mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
      mailMessage.Body = _message;
      mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
      mailMessage.IsBodyHtml = true;
      mailMessage.Priority = MailPriority.Normal;

      return mailMessage;
    }

    public void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
      // Get the unique identifier for this asynchronous operation.
      String token = (string)e.UserState;

      // TODO: Внести логирование
      if (e.Cancelled)
      {
        State = MailState.Canceled;
      }
      if (e.Error != null)
      {
        State = MailState.Error;
      }
      else
      {
        //Async send is completed
      }
      State = MailState.Sended;
    }
  }
}