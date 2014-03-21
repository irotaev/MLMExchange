using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MLMExchange.Lib
{
  /// <summary>
  /// Валидация. Расширение HtmlHelper
  /// </summary>
  public static class ValidationExtensions
  {
    public static MvcHtmlString CustomValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
    {
      TagBuilder arrowTag = new TagBuilder("div");
      arrowTag.AddCssClass("b-ib__validation-message--arrow");

      MvcHtmlString validationMessage = htmlHelper.ValidationMessageFor(expression, null, new { @class = "b-ib__validation-message" });

      return new MvcHtmlString(validationMessage + arrowTag.ToString());
    }
  }
}
