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
using MLMExchange.Lib;
using MLMExchange.Models.Registration;
using NHibernate.Linq;
using MLMExchange.Lib.Exception;
using Logic.Lib;

namespace MLMExchange.Areas.AdminPanel.Controllers
{
  [Auth(typeof(D_UserRole), typeof(D_AdministratorRole))]
  public class BaseUserRoleController : BaseController, IDataObjectCustomizableController<BaseRoleModel, BaseBrowseActionSettings, BaseEditActionSettings, BaseUserRoleController._ListActionSettings>
  {
    public ActionResult Browse(BaseBrowseActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult Edit(BaseRoleModel model, BaseEditActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    public ActionResult List(_ListActionSettings actionSettings)
    {
      throw new NotImplementedException();
    }

    #region User role access level
    /// <summary>
    /// Получить все уровни доступа для ролей системы
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public JsonResult GetAllRoleTypeAccessLevels()
    {
      IEnumerable<RoleTypeAccessLevelModel> models = RoleTypeAccessLevel.GetAllRoleTypeAccessLevels().Select(x => new RoleTypeAccessLevelModel().Bind((RoleTypeAccessLevel)x));

      return Json(models, JsonRequestBehavior.AllowGet);
    }

    [Auth(typeof(D_AdministratorRole))]
    [HttpPut]
    public void UpdateAllRoleTypeAccessLevels(List<RoleTypeAccessLevelModel> accessRoleModels)
    {
      ModelState.Clear();
      TryUpdateModel<List<RoleTypeAccessLevelModel>>(accessRoleModels);

      if (!ModelState.IsValid)
        throw new UserVisible__CurrentActionAccessDenied();

      foreach(var model in accessRoleModels)
      {
        if (model.IsTradeEnabled == null)
          continue;

        D_RoleTypeAccessLevel accessLevel = _NHibernateSession.Query<D_RoleTypeAccessLevel>().Where(x => x.Id == model.Id).FirstOrDefault();

        if (accessLevel == null)
          throw new UserVisible__WrongParametrException("roleId");

        if (!((RoleTypeAccessLevel)accessLevel).TryChangeAccessLevel(model.IsTradeEnabled.Value))
          throw new UserVisibleException(MLMExchange.Properties.ResourcesA.UpdateAllRoleTypeAccessLevels__Exception_CannotChangeLevel);
      }
    }
    #endregion

    /// <summary>
    /// Получить все роли для конкретного пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <returns></returns>
    [HttpGet]
    public JsonResult GetAllUserRoles(long userId)
    {
      D_User user = _NHibernateSession.Query<D_User>().Where(x => x.Id == userId).FirstOrDefault();

      if (user == null)
        throw new UserVisible__WrongParametrException("UserId");

      List<BaseRoleModel> models = new List<BaseRoleModel>();
      models.AddRange(user.Roles.Select(x => new BaseRoleModel().Bind(x)));

      return Json(Newtonsoft.Json.JsonConvert.SerializeObject(models), JsonRequestBehavior.AllowGet);
    }    

    #region Add user role
    [Auth(typeof(D_AdministratorRole))]
    [HttpPost]
    public void AddUserRoles(long userId, [Bind(Exclude = "TextResources")] List<BaseRoleModel> roleTypes)
    {
      D_User user = _NHibernateSession.Query<D_User>().Where(x => x.Id == userId).FirstOrDefault();

      if (user == null)
        throw new UserVisible__WrongParametrException("userId");

      if (roleTypes == null)
        throw new UserVisible__WrongParametrException("roleTypes");

      foreach(var roleType in roleTypes)
      {
        if (roleType.Id != null && roleType.Id != 0)
          continue;

        AddUserRole(user, roleType.RoleType);
      }
    }

    private void AddUserRole(D_User user, RoleType roleType)
    {
      switch(roleType)
      {
        case RoleType.Administrator:
          if (((Logic.User)user).GetRole<D_AdministratorRole>() != null)
            throw new UserVisibleException(MLMExchange.Properties.ResourcesA.UserAlreadyContainsThisRoleType);

          user.Roles.Add(new D_AdministratorRole { User = user });
          break;
        case RoleType.Broker:
          if (((Logic.User)user).GetRole<D_BrokerRole>() != null)
            throw new UserVisibleException(MLMExchange.Properties.ResourcesA.UserAlreadyContainsThisRoleType);

          user.Roles.Add(new D_BrokerRole { User = user });
          break;
        case RoleType.Leader:
          if (((Logic.User)user).GetRole<D_LeaderRole>() != null)
            throw new UserVisibleException(MLMExchange.Properties.ResourcesA.UserAlreadyContainsThisRoleType);

          user.Roles.Add(new D_LeaderRole { User = user });
          break;
        case RoleType.Tester:
          if (((Logic.User)user).GetRole<D_TesterRole>() != null)
            throw new UserVisibleException(MLMExchange.Properties.ResourcesA.UserAlreadyContainsThisRoleType);

          user.Roles.Add(new D_TesterRole { User = user });
          break;
        case RoleType.User:
          if (((Logic.User)user).GetRole<D_UserRole>() != null)
            throw new UserVisibleException(MLMExchange.Properties.ResourcesA.UserAlreadyContainsThisRoleType);

          user.Roles.Add(new D_UserRole { User = user });
          break;
      }

      _NHibernateSession.SaveOrUpdate(user);
    }
    #endregion

    [Auth(typeof(D_AdministratorRole))]
    [HttpDelete]
    public void RemoveUserRoles(long userId, [Bind(Exclude = "TextResources")] List<BaseRoleModel> roleTypes)
    {
      D_User user = _NHibernateSession.Query<D_User>().Where(x => x.Id == userId).FirstOrDefault();

      if (user == null)
        throw new UserVisible__WrongParametrException("userId");

      if (roleTypes == null)
        throw new UserVisible__WrongParametrException("roleTypes");

      foreach (var roleModel in roleTypes)
      {
        if (roleModel.Id == null || roleModel.Id == 0)
          continue;

        if (roleModel.RoleType == RoleType.Administrator)
          throw new UserVisibleException(MLMExchange.Properties.ResourcesA.CanNotRemoveAdministratorRolFromUser);

        D_AbstractRole role = user.Roles.Where(x => x.Id == roleModel.Id).FirstOrDefault();

        if (role == null)
          continue;

        user.Roles.Remove(role);
        _NHibernateSession.SaveOrUpdate(user);
      }
    }

    public class _ListActionSettings : BaseListActionSetings
    {
      /// <summary>
      /// Id пользователя, у которого брать роли
      /// </summary>
      public long UserId { get; set; }
    }
  }
}