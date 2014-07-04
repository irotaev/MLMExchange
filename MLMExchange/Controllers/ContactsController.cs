using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MLMExchange.Models;
using MLMExchange.Lib;

namespace MLMExchange.Controllers
{
  public class ContactsController : BaseController
  {
    public ActionResult Index()
    {
      return View();
    }

    /// <summary>
    /// Обработчик формы "Контакты"
    /// </summary>
    /// <param name="contactModel"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult SendMail(ContactMessageModel contactModel)
    {
      MailSender mail = new MailSender(contactModel.EMail, contactModel.UserName, contactModel.Title, contactModel.Text);
      mail.SendMailMessage();
      return null;
    }
  }
}
