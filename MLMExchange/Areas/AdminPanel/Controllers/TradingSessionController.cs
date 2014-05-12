using MLMExchange.Areas.AdminPanel.Models;
using MLMExchange.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  public class TradingSessionController : BaseController, IDataObjectCustomizableController<TradingSessionModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseListActionSetings>
  {
    public System.Web.Mvc.ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public System.Web.Mvc.ActionResult Edit(TradingSessionModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }


    public System.Web.Mvc.ActionResult List(BaseListActionSetings actionSettings)
    {
      throw new NotImplementedException();
    }
  }
}