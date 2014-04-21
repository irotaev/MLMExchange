using Logic;
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
        model = UserModel.Bind(user);
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

              user = UserModel.UnBind(model, user);

              session.SaveOrUpdate(user);
              transaction.Commit();
            }
          }
        }
      }

      return View(model);
    }
  }
}