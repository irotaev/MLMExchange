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
    [HiddenInput(DisplayValue = false)]
    public long Id { get; set; }

    [Required(ErrorMessage = "Введите пожалуйста логин")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Введите пожалуйста парооль")]
    public string PasswordHash { get; set; }
  }
}