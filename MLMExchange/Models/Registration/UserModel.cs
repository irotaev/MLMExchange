using Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using NHibernate.Linq;
using DataAnnotationsExtensions;
using MLMExchange.Lib.Exception;
using MLMExchange.Areas.AdminPanel.Models;

namespace MLMExchange.Models.Registration
{
  /// <summary>
  /// Модель пользователя
  /// </summary>
  public class UserModel : AbstractDataModel<D_User, UserModel>
  {
    public UserModel()
    {
      _LazyLoadProperties.Add("ReferalRoleId", false);
    }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Login { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [StringLength(50, MinimumLength = 8, ErrorMessageResourceName = "FieldMinCharCountNotRequired_8", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [System.Web.Mvc.Compare("Password", ErrorMessageResourceName = "PasswordDoesnotMatchWithPasswordConfirmation", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [StringLength(50, MinimumLength = 8, ErrorMessageResourceName = "FieldMinCharCountNotRequired_8", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [DataType(DataType.Password)]
    public string PasswordConfirmation { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Name { get; set; }

    public string Surname { get; set; }

    public string Patronymic { get; set; }

    [Required(ErrorMessageResourceName = "FieldFilledInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessageResourceName = "FieldEmailInvalid", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public string Email { get; set; }

    #region ReferalRoleId
    private long? _ReferalRoleId;

    [Integer(ErrorMessageResourceName = "FieldFilledInvalid_IntegerOnly", ErrorMessageResourceType = typeof(MLMExchange.Properties.ResourcesA))]
    public long? ReferalRoleId
    {
      get
      {
        if (_LazyLoadProperties["ReferalRoleId"] == true)
        {
          if (_Object == null)
            throw new BindNotCallException<User>();

          if (_Object.RefererRole == null)
          {
            return null;
          }
          else
          {
            return _Object.RefererRole.Id;
          }
        }
        else
        {
          return _ReferalRoleId;
        }
      }

      set
      {
        _ReferalRoleId = value;
      }
    }

    /// <summary>
    /// Получить Id реферера из запроса
    /// </summary>
    public static long? GetRefererRoleIdFromRequest(HttpRequestBase request)
    {
      if (request == null)
        throw new Logic.Lib.ApplicationException("request is null");

      long? refererId = null;

      {
        var keys = request.QueryString.GetValues("RefererId");
        if (keys != null && keys.Length >= 1)
        {
          long id;
          if (Int64.TryParse(keys[0], out id))
          {
            refererId = id;
          }
        }
      }

      return refererId;
    }
    #endregion

    public HttpPostedFileBase Photo { get; set; }
    [HiddenInput(DisplayValue = false)]
    public string PhotoRelativePath { get; set; }

    public IEnumerable<D_AbstractRole> UserRoles { get; set; }
    public string DisplayName { get; set; }

    /// <summary>
    /// Получить роль для текущего пользователя.
    /// Если данной роли у пользователя нету, то вернет null.
    /// </summary>
    /// <typeparam name="TRoleModel"></typeparam>
    /// <returns></returns>
    public TRoleModel GetUserRole<TRoleModel>()
      where TRoleModel : class, MLMExchange.Areas.AdminPanel.Models.IAbstractRoleModel
    {
      if (_Object == null)
        throw new BindNotCallException<User>();

      Type roleModelType = typeof(TRoleModel);

      //TODO:Rtv дореализовать все роли
      if (roleModelType == typeof(UserRoleModel))
      {
        D_UserRole role = ((User)_Object).GetRole<D_UserRole>();

        if (role == null)
          return null;

        return new UserRoleModel().Bind(role) as TRoleModel;
      }

      return null;
    }

    public UserModel Bind(Logic.D_User @object)
    {
      if (@object == null)
        throw new ArgumentNullException("user");

      base.Bind(@object);

      this.Email = @object.Email;
      this.Login = @object.Login;
      this.Name = @object.Name;
      this.Patronymic = @object.Patronymic;
      this.PhotoRelativePath = @object.PhotoRelativePath;
      this.Surname = @object.Surname;

      UserRoles = @object.Roles;

      DisplayName = String.Format("{0} {1}", Name, Surname);

      return this;
    }

    #region UnBind
    private D_User UnBindLogic(D_User user = null)
    {
      if (user == null)
        user = new D_User();

      user.Login = this.Login;
      user.PasswordHash = MLMExchange.Lib.Md5Hasher.ConvertStringToHash(this.Password);
      user.Email = this.Email;
      user.Name = this.Name;
      user.Surname = this.Surname;
      user.Patronymic = this.Patronymic;
      user.PhotoRelativePath = this.PhotoRelativePath;

      if (_ReferalRoleId != null)
      {
        if (user.Id != 0)
          throw new Logic.Lib.UserVisibleException(MLMExchange.Properties.PrivateResource.UserModel__Exception_CantChangeRefererRoleAfterRegistration);

        D_UserRole referalRole = _NhibernateSession.Query<D_UserRole>().Where(x => x.Id == _ReferalRoleId).FirstOrDefault();

        if (referalRole == null)
          throw new UserVisible__WrongParametrException("referalId");

        user.RefererRole = referalRole;
      }

      if (user.PaymentSystemGroup == null)
        user.PaymentSystemGroup = new D_PaymentSystemGroup();

      return user;
    }

    /// <summary>
    /// Анбиндинг в логический объект. Прокси-анбиндинг
    /// </summary>
    /// <returns>Логический объект</returns>
    public D_User UnBind()
    {
      return UnBindLogic();
    }

    /// <summary>
    /// Анбиндинг в логический объект. Прокси-анбиндинг
    /// </summary>
    /// <param name="object">Логический объект User. В него произойдет анбиндинг</param>
    /// <returns>Логический объект</returns>
    public D_User UnBind(D_User @object)
    {
      return UnBindLogic(@object);
    }
    #endregion
  }
}