using Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Models.Registration
{
  public class UserModel : AbstractDataModel, IDataBinding<User>
  {    
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

    public void Bind(Logic.User @object)
    {
      if (@object == null)
        throw new ArgumentNullException("user");

      this.Id = @object.Id;
      this.Email = @object.Email;
      this.Login = @object.Login;
      this.Name = @object.Name;
      this.Patronymic = @object.Patronymic;
      this.PhotoRelativePath = @object.PhotoRelativePath;
      this.Surname = @object.Surname;
    }

    #region UnBind
    private User UnBindLogic(User user = null)
    {
      if (user == null)
        user = new User();

      user.Login = this.Login;
      user.PasswordHash = MLMExchange.Lib.Md5Hasher.ConvertStringToHash(this.Password);
      user.Email = this.Email;
      user.Name = this.Name;
      user.Surname = this.Surname;
      user.Patronymic = this.Patronymic;
      user.PhotoRelativePath = this.PhotoRelativePath;

      return user;
    }

    /// <summary>
    /// Анбиндинг в логический объект. Прокси-анбиндинг
    /// </summary>
    /// <returns>Логический объект</returns>
    public User UnBind()
    {
      return UnBindLogic();
    }

    /// <summary>
    /// Анбиндинг в логический объект. Прокси-анбиндинг
    /// </summary>
    /// <param name="object">Логический объект User. В него произойдет анбиндинг</param>
    /// <returns>Логический объект</returns>
    public User UnBind(User @object)
    {
      return UnBindLogic(@object);
    }
    #endregion
  }
}