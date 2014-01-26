﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MLMExchange.Models
{
  public class LoginModel : BaseModel
  {
    [Required(ErrorMessage="*")]
    public string Login { get; set; }

    [Required(ErrorMessage="*")]
    public string Password { get; set; }
  }
}