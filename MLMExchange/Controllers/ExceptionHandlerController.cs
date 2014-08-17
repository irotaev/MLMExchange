using MLMExchange.Models.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logic.Lib;

namespace MLMExchange.Controllers
{
  /// <summary>
  /// Контроллер отображения ошибок
  /// </summary>
  public class ExceptionHandlerController : BaseController
  {
    public ActionResult ApplicationException()
    {
      Logic.Application.NLogLogger.Info("Заход в контроллер ошибок (ExceptionHandlerController)");
      var model = new ApplicationExceptionModel();

      #region Обработка ошибки      
      model.ExceptionMessage = MLMExchange.Properties.PrivateResource.Application__Exception_ServerException;

      System.Web.HttpContext.Current.ClearError();
      #endregion

      return View(model);
    }
  }
}