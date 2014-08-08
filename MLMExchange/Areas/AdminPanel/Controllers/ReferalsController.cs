using Logic;
using MLMExchange.Areas.AdminPanel.Models;
using MLMExchange.Controllers;
using MLMExchange.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using NHibernate.Linq;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  /// <summary>
  /// Контроллер рефералов
  /// </summary>
  [Auth(typeof(D_UserRole))]
  public class ReferalsController : BaseController
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult Edit(ReferalModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult List(BaseListActionSetings actionSettings)
    {
      ReferalListModel model = new ReferalListModel().Bind((User)CurrentSession.Default.CurrentUser);

      IEnumerable<D_User> referals = _NHibernateSession.Query<D_User>().Where(x => x.RefererRole.Id == ((User)CurrentSession.Default.CurrentUser).GetRole<D_UserRole>().Id);

      foreach(var referal in referals)
      {
        ReferalModel referalModel = new ReferalModel
        {
          Referal = new MLMExchange.Models.Registration.UserModel().Bind(referal),
          RefererTotalrofit = ((UserRole)((Logic.User)referal).GetRole<D_UserRole>()).CalculateTotalRefererProfit()
        };

        model.Referals.Add(referalModel);
      }

      return View(model);
    }
  }
}
