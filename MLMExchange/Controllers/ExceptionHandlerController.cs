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
      var model = new ApplicationExceptionModel();

      #region Обработка ошибки
      Exception ex = TempData["Controller__Exception"] as Exception;

      if (ex != null)
      {
        model.ExceptionMessage = ex.GetAllExceptionTreeLog("<br/><br/>");
      }

      System.Web.HttpContext.Current.ClearError();
      #endregion

      return View(model);
    }
  }
}