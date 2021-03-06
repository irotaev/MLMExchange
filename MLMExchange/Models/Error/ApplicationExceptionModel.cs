﻿using MLMExchange.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLMExchange.Models.Error
{
  /// <summary>
  /// Ошибка системы
  /// </summary>
  public class ApplicationExceptionModel : AbstractModel
  {
    public ApplicationExceptionModel(string title = null)
    {
      if (String.IsNullOrEmpty(title))
        ExceptionTitle = MLMExchange.Properties.ResourcesA.ApplicationExceptionOccupied;
    }

    private string _ExceptionMessage;

    public string ExceptionTitle { get; set; }
    public virtual string ExceptionMessage
    {
      get
      {
        if (String.IsNullOrEmpty(_ExceptionMessage))
          _ExceptionMessage = MLMExchange.Properties.ResourcesA.DataEmpty;

        return _ExceptionMessage;
      }

      set { _ExceptionMessage = value; }
    }
  }

  /// <summary>
  /// Ошибка контроллера
  /// </summary>
  public class ControllerExceptionModel : ApplicationExceptionModel
  {
    public ControllerExceptionModel(BaseController controller, string title = null) : base(title)
    {
      _Controller = controller;
    }

    public ControllerExceptionModel(BaseController controller, Exception ex, string title = null)
      : this(controller, title)
    {
      _Exception = ex;
    }

    protected readonly BaseController _Controller;
    protected readonly Exception _Exception;
  }
}