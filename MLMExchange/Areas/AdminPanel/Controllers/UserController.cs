using Logic;
using MLMExchange.Areas.AdminPanel.Models.User;
using MLMExchange.Controllers;
using MLMExchange.Lib;
using MLMExchange.Lib.Exception;
using MLMExchange.Models;
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

    /// <summary>
    /// Добавить/пополнить my-crypt
    /// </summary>
    /// <param name="model">Модель добавления myc-rypt</param>
    /// <returns></returns>
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

    /// <summary>
    /// Панель управления пользователя
    /// </summary>
    /// <returns></returns>
    public ActionResult ControlPanel()
    {
      return View();
    }

    /// <summary>
    /// Подать заявку на участие в торгах
    /// </summary>
    /// <param name="model">Модель подачи заявки</param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult BiddingParticipateApplicationApply(BiddingParticipateApplicationModel model)
    {
      ModelState.Clear();

      UpdateModel<BiddingParticipateApplicationModel>(model);

      if (ModelState.IsValid)
      {
        using (var session = NHibernateConfiguration.Session.OpenSession())
        {
          using (var transaction = session.BeginTransaction())
          {
            BiddingParticipateApplication biddingApplication = model.UnBind((BiddingParticipateApplication)null);

            session.Save(biddingApplication);
            transaction.Commit();
          }
        }
      }
      else
      {
        throw new UserVisibleException(MLMExchange.Properties.ResourcesA.Exception_ModelInvalid);
      }

      return Redirect(Request.UrlReferrer.ToString());
    }

    /// <summary>
    /// Продавцы
    /// </summary>
    /// <returns></returns>
    public ActionResult SalesPeople()
    {
      IList<BiddingParticipateApplication> biddingApplications;

      using (var session = NHibernateConfiguration.Session.OpenSession())
      {
        biddingApplications = session.QueryOver<BiddingParticipateApplication>().List();
      }

      return View(biddingApplications);
    }
  }
}