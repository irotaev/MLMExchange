using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Lib.DataValidation
{
  /// <summary>
  /// Атрибут проверки модели данных.
  /// Проверяет на условие типа bool
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public class BooleanAttribute : ValidationAttribute, IClientValidatable
  {
    public bool ValidState { get; set; }

    public override bool IsValid(object value)
    {
      return (value as bool?).GetValueOrDefault();
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      yield return new ModelClientValidationRule
      {
        ValidationType = "boolean",
        ErrorMessage = this.ErrorMessageString
      };
    }
  }
}