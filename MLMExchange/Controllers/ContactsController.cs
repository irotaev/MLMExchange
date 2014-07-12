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
    /// <returns>Redirect to success page</returns>
    [HttpPost]
    public ActionResult SendMail(ContactMessageModel contactModel)
    {
      Mail mail = new Mail(contactModel.EMail, contactModel.UserName, contactModel.Title, contactModel.Text);
      mail.SendMailMessage();
      return Redirect("/Contacts/Success");
    }

    /// <summary>
    /// Экшн успешной отправки сообщения
    /// </summary>
    /// <returns>view</returns>
    public ActionResult Success()
    {
      return View();
    }
  }
}
