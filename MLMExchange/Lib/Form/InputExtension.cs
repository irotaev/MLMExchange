using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MLMExchange.Lib;
using System.Linq.Expressions;

namespace MLMExchange.Lib.Form
{
  /// <summary>
  /// Расширение для Html.TexBox и т.д.
  /// </summary>
  public static class InputExtension
  {
    /// <summary>
    /// Кастомный TextBoxFor для данного проекта
    /// </summary>
    /// <typeparam name="TModel">Тип модели представления</typeparam>
    /// <param name="fieldDiscription">Описание к текушему полю</param>
    /// <param name="fieldDisplatyName">Отображаемое название поля</param>
    /// <param name="fieldExpression">Expression для текущего поля модели</param>
    /// <param name="formStyle">Стиль формы</param>
    /// <returns></returns>
    public static MvcHtmlString ApplicationTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
      Expression<Func<TModel, TProperty>> fieldExpression, string fieldDisplatyName, string fieldDiscription = null, string formStyle = "blue")
    {
      fieldDiscription = fieldDiscription ?? fieldDisplatyName;

      TagBuilder inputBlockTag = new TagBuilder("div");
      inputBlockTag.AddCssClass("b-ib input-control text");

      inputBlockTag.InnerHtml = htmlHelper.CustomValidationMessageFor(fieldExpression).ToString();

      TagBuilder ibContentWrapperTag = new TagBuilder("div");
      ibContentWrapperTag.AddCssClass("b-ib__content-wrapper");

      #region Info helper
      TagBuilder ibInfoHelper = new TagBuilder("div");
      ibInfoHelper.AddCssClass("b-ib__info-helper");
      ibInfoHelper.MergeAttribute("data-ot", fieldDiscription);
      ibInfoHelper.MergeAttribute("data-ot-style", "BlackStyle");

      TagBuilder ibInfoIcon = new TagBuilder("i");
      ibInfoIcon.AddCssClass("b-ib__info-icon fa fa-question");

      ibInfoHelper.InnerHtml = ibInfoIcon.ToString();

      ibContentWrapperTag.InnerHtml = ibInfoHelper.ToString();
      #endregion

      #region Input content
      TagBuilder ibInputContent = new TagBuilder("div");
      ibInputContent.AddCssClass("b-ib__input-content");

      TagBuilder ibLabel = new TagBuilder("label");
      ibLabel.AddCssClass("b-ib__label");
      ibLabel.InnerHtml = fieldDisplatyName;

      TagBuilder clearBtnTag = new TagBuilder("button");
      clearBtnTag.AddCssClass("btn-clear");

      ibInputContent.InnerHtml = ibLabel.ToString()
        + htmlHelper.TextBoxFor(fieldExpression, new { placeholder = fieldDisplatyName, @class = String.Format("b-ib__input b-ib__input_type_{0}", formStyle) }).ToString()
        + clearBtnTag.ToString();

      ibContentWrapperTag.InnerHtml += ibInputContent.ToString();
      #endregion

      inputBlockTag.InnerHtml += ibContentWrapperTag.ToString();

      return MvcHtmlString.Create(inputBlockTag.ToString());
    }
  }
}