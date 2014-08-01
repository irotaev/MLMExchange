using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MLMExchange.Models;
using MLMExchange.Lib;
using Logic.Lib;

namespace MLMExchange.Controllers
{
  public class ContactsController : BaseController
  {
    public ActionResult Index(string redirectUrl = null)
    {
      ModelState.Clear();
      ContactMessageModel contactModel = new ContactMessageModel();

      if (ControllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        TryUpdateModel<ContactMessageModel>(contactModel);

        if (ModelState.IsValid)
        {
          #region Validation
          if (String.IsNullOrEmpty(contactModel.UserName))
            throw new UserVisible__ArgumentNullException("UserName");

          if (String.IsNullOrEmpty(contactModel.Email))
            throw new UserVisible__ArgumentNullException("EMail");

          if (String.IsNullOrEmpty(contactModel.Title))
            throw new UserVisible__ArgumentNullException("Title");

          if (String.IsNullOrEmpty(contactModel.Text))
            throw new UserVisible__ArgumentNullException("Text");
          #endregion

          Mail mail = new Mail(contactModel.Email, contactModel.UserName, contactModel.Title, contactModel.Text);
          mail.SendMailMessage();

          return RedirectToAction("Success", "Contacts", new
          {
            Type = RedirectType.SuccessSendMail
          });
        }
      }


      return View(contactModel);
    }

    /// <summary>
    /// Обработчик формы "Контакты"
    /// </summary>
    /// <param name="contactModel"></param>
    /// <returns>Redirect to success page</returns>
    [HttpPost]
    public ActionResult SendMail(ContactMessageModel contactModel)
    {
      ModelState.Clear();

      #region Validation
      if (String.IsNullOrEmpty(contactModel.UserName))
        throw new UserVisible__ArgumentNullException("UserName");

      if (String.IsNullOrEmpty(contactModel.Email))
        throw new UserVisible__ArgumentNullException("EMail");

      if (String.IsNullOrEmpty(contactModel.Title))
        throw new UserVisible__ArgumentNullException("Title");

      if (String.IsNullOrEmpty(contactModel.Text))
        throw new UserVisible__ArgumentNullException("Text");
      #endregion

      TryUpdateModel<ContactMessageModel>(contactModel);

      if (ModelState.IsValid)
      { 
        Mail mail = new Mail(contactModel.Email, contactModel.UserName, contactModel.Title, contactModel.Text);
        mail.SendMailMessage();
      }

      
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
