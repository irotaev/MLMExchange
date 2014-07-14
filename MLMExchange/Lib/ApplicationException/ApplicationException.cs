using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Logic.Lib;

namespace MLMExchange.Lib.Exception
{
  /// <summary>
  /// Данное действие контролера может быть вызвано только посредством Ajax-запросов (коротких запросов)
  /// </summary>
  public class UserVisible__ActionAjaxOnlyException : UserVisibleException
  {
    public UserVisible__ActionAjaxOnlyException()
      : base()
    {
      throw new UserVisibleException(MLMExchange.Properties.ResourcesA.Exception_ActionAjaxOnly);
    }
  }  

  public class UserVisible__WrongParametrException : UserVisibleException
  {
    public UserVisible__WrongParametrException(string argumentName)
      : base() 
    {
      throw new UserVisibleException(String.Format(MLMExchange.Properties.ResourcesA.Exception_WrongParameter, argumentName));
    }
  }

  public class UserVisible__CurrentActionAccessDenied : UserVisibleException
  {
    public UserVisible__CurrentActionAccessDenied()
      : base()
    {
      throw new UserVisibleException(MLMExchange.Properties.ResourcesA.YouDontHaveAccessToThisAction);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionDescription">Описание действия</param>
    public UserVisible__CurrentActionAccessDenied(string actionDescription)
      : base()
    {
      throw new UserVisibleException(String.Format(MLMExchange.Properties.ResourcesA.YouDontHaveAccessToThisAction_WithActionDescription, actionDescription));
    }
  }
}