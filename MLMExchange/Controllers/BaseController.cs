using Logic;
using MLMExchange.Models.Registration;
using MLMExchange.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;

namespace MLMExchange.Controllers
{
  public abstract class BaseController : Controller
  {
    protected HtmlHelper _HtmlHelper;

    public BaseController()
    {
      //ViewData["AdminPanel__CenterBlock"] = 
    }

    protected override void Initialize(System.Web.Routing.RequestContext requestContext)
    {
      base.Initialize(requestContext);

      #region Модель для логина
      LoginModel loginModel = new LoginModel();

      TryUpdateModel<LoginModel>(loginModel);

      ViewBag.LoginModel = loginModel;
      #endregion

      #region Добавляю данные в ViewData
      _HtmlHelper = new HtmlHelper(new ViewContext(ControllerContext, new WebFormView(ControllerContext, "FakeView"), new ViewDataDictionary(), new TempDataDictionary(), new System.IO.StringWriter()), new ViewPage());

      ViewData["AdminPanel__CenterBlock"] = _HtmlHelper.X().Panel().ID("AdminPanel__CenterBlock").Region(Region.Center);
      #endregion
    }
  }
}
