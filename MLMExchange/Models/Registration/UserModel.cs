using Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Models.Registration
{
  public class UserModel : BaseModel
  {
    [HiddenInput]
    public long? Id { get; set; }

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

    public HttpPostedFileBase Photo { get; set; }
    [HiddenInput(DisplayValue = false)]
    public string PhotoRelativePath { get; set; }

    /// <summary>
    /// Биндинг в web-модель. Прокси-биндинг
    /// </summary>
    /// <param name="user">Логическая модель</param>
    /// <returns>Web-модель</returns>
    public static UserModel Bind(Logic.User user)
    {
      if (user == null)
        throw new ArgumentNullException("user");

      UserModel userModel = new UserModel
      {
        Id = user.Id,
        Email = user.Email,
        Login = user.Login,
        Name = user.Name,
        Patronymic = user.Patronymic,
        PhotoRelativePath = user.PhotoRelativePath,
        Surname = user.Surname
      };

      return userModel;
    }

    #region UnBind
    private static User UnBindLogic(UserModel userModel, User user = null)
    {
      if (userModel == null)
        throw new ArgumentNullException("userModel");

      if (user == null)
        user = new User();

      user.Login = userModel.Login;
      user.PasswordHash = MLMExchange.Lib.Md5Hasher.ConvertStringToHash(userModel.Password);
      user.Email = userModel.Email;
      user.Name = userModel.Name;
      user.Surname = userModel.Surname;
      user.Patronymic = userModel.Patronymic;
      user.PhotoRelativePath = userModel.PhotoRelativePath;

      return user;
    }

    /// <summary>
    /// Анбиндинг в логический объект. Прокси-анбиндинг
    /// </summary>
    /// <param name="userModel">Web-модель</param>
    /// <returns>Логический объект</returns>
    public static User UnBind(UserModel userModel)
    {
      return UnBindLogic(userModel);
    }

    /// <summary>
    /// Анбиндинг в логический объект. Прокси-анбиндинг
    /// </summary>
    /// <param name="userModel">Web-модель</param>
    /// <param name="user">Логический объект User. В него произойдет анбиндинг</param>
    /// <returns>Логический объект</returns>
    public static User UnBind(UserModel userModel, User user)
    {
      return UnBindLogic(userModel, user);
    }
    #endregion
  }
}