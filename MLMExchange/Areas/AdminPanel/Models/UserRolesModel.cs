using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MLMExchange.Controllers;
using MLMExchange.Areas.AdminPanel.Models;
using Logic;
using Ext.Net;
using Ext.Net.MVC;
using MLMExchange.Models;
using MLMExchange.Models.Registration;

namespace MLMExchange.Areas.AdminPanel.Models
{
  public class UserRolesModel : AbstractDataModel
  {

    public static Paging<UserModel> UserPaging(StoreRequestParameters parameters)
    {
      return UserPaging(parameters.Start, parameters.Limit, parameters.SimpleSort, parameters.SimpleSortDirection, null);
    }

    public static Paging<UserModel> UserPaging(int start, int limit, string sort, SortDirection dir, string filter)
    {
      List<UserModel> userList = UserRoles.GetUsersWithPaging(start, limit).Select(x => new UserModel().Bind(x)).ToList();

      if ((start + limit) > userList.Count)
      {
        limit = userList.Count - start;
      }

      List<UserModel> rangePlants = (start < 0 || limit < 0) ? userList : userList.GetRange(start, limit);

      return new Paging<UserModel>(rangePlants, userList.Count);
    }
  }
}