using MLMExchange.Models.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Controllers
{
  /// <summary>
  /// Контроллер отображения ошибок
  /// </summary>
  public class ExceptionHandlerController : BaseController
  {
    [HttpGet]
    public ViewResult ApplicationException()
    {
      var model = new ApplicationExceptionModel();

      return View(model);
    }
  }
}