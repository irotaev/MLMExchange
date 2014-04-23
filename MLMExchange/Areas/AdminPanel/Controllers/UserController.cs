using Logic;
using MLMExchange.Areas.AdminPanel.Models.User;
using MLMExchange.Controllers;
using MLMExchange.Lib;
using MLMExchange.Models.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  [Auth]
  public class UserController : BaseController
  {
    /// <summary>
    /// Редактирование информации о пользователе
    /// </summary>
    /// <param name="model">Модель пользователя</param>
    /// <returns></returns>
    public ActionResult Edit(UserModel model)
    {
      Logic.User user = CurrentSession.Default.CurrentUser;

      if (model.Id == null)
      {
        model.Bind(user);
      }
      else if (ControllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        if (ModelState.IsValid)
        {
          using (var session = NHibernateConfiguration.Session.OpenSession())
          {
            using (var transaction = session.BeginTransaction())
            {
              #region Сохраняю фото
              if (model.Photo != null)
              {
                string filePath = MLMExchange.Lib.Image.Image.SaveImage(model.Photo, Server);

                model.PhotoRelativePath = filePath;
              }
              #endregion

              user = model.UnBind(user);

              session.SaveOrUpdate(user);
              transaction.Commit();
            }
          }
        }
      }

      return View(model);
    }

    public ActionResult AddMyCrypt(AddMyCryptModel model)
    {
      ModelState.Clear();

      if (ControllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        if (ModelState.IsValid)
        {
          using (var session = NHibernateConfiguration.Session.OpenSession())
          {
            using (var transaction = session.BeginTransaction())
            {
              #region Сохраняю фото
              if (model.Image != null)
              {
                string filePath = MLMExchange.Lib.Image.Image.SaveImage(model.Image, Server);

                model.ImageRelativePath = filePath;
              }
              #endregion

              AddMyCryptTransaction addMyCryptTransaction = model.UnBind();

              session.Save(addMyCryptTransaction);
              transaction.Commit();
            }
          }

          return View("_AddMyCrypt_Success", model);
        }
      }

      return View(model);
    }
  }
}