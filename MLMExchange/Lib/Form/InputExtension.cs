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
    private static MvcHtmlString RenderApplicationTextBoxFor(string inputBlockTag_InnerHtml, string textBoxForAsString, string fieldDisplatyName, string fieldDiscription = null, string formStyle = "blue", bool isNeedClearBtn = true)
    {
      fieldDiscription = fieldDiscription ?? fieldDisplatyName;

      TagBuilder inputBlockTag = new TagBuilder("div");
      inputBlockTag.AddCssClass("b-ib input-control text");

      inputBlockTag.InnerHtml = inputBlockTag_InnerHtml;

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

      ibInputContent.InnerHtml = ibLabel.ToString()
        + textBoxForAsString;

      if (isNeedClearBtn)
      {
        TagBuilder clearBtnTag = new TagBuilder("button");
        clearBtnTag.AddCssClass("btn-clear");

        ibInputContent.InnerHtml += clearBtnTag.ToString();
      }

      ibContentWrapperTag.InnerHtml += ibInputContent.ToString();
      #endregion

      inputBlockTag.InnerHtml += ibContentWrapperTag.ToString();

      return MvcHtmlString.Create(inputBlockTag.ToString());
    }

    private static bool IsNeedBtnClear(IDictionary<string, object> additionalAttributes)
    {
      bool isNeedBtnClear = true;

      object readonlyAttr;
      if (additionalAttributes.TryGetValue("readonly", out readonlyAttr) || additionalAttributes.TryGetValue("disabled", out readonlyAttr))
        isNeedBtnClear = false;

      return isNeedBtnClear;
    }

    /// <summary>
    /// Кастомный TextBoxFor для данного проекта
    /// </summary>
    /// <typeparam name="TModel">Тип модели представления</typeparam>
    /// <param name="fieldDiscription">Описание к текушему полю</param>
    /// <param name="fieldDisplatyName">Отображаемое название поля</param>
    /// <param name="fieldExpression">Expression для текущего поля модели</param>
    /// <param name="formStyle">Стиль формы</param>
    /// <param name="value">Значение для поля</param>
    /// <param name="additionalAttributes">Дополнительные аттрибуты к input</param>
    /// <returns></returns>
    public static MvcHtmlString ApplicationTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
      Expression<Func<TModel, TProperty>> fieldExpression, string fieldDisplatyName, string fieldDiscription = null, string formStyle = "blue", string value = null, IDictionary<string, object> additionalAttributes = null)
    {
      string inputBlockTag_InnerHtml = htmlHelper.CustomValidationMessageFor(fieldExpression).ToString();

      if (additionalAttributes == null)
        additionalAttributes = new Dictionary<string, object>();

      Dictionary<string, object> additionalParams = new Dictionary<string, object>(additionalAttributes)
      {
        {"placeholder", fieldDisplatyName},
        {"class", String.Format("b-ib__input b-ib__input_type_{0}", formStyle)}
      };

      if (value != null)
        additionalParams.Add("value", value);

      string textBoxForAsString = htmlHelper.TextBoxFor(fieldExpression, additionalParams).ToString();

      return RenderApplicationTextBoxFor(inputBlockTag_InnerHtml, textBoxForAsString, fieldDisplatyName, fieldDiscription, formStyle, IsNeedBtnClear(additionalParams));
    }

    /// <summary>
    /// Кастомный TextBoxFor для данного проекта.
    /// Нету валидации.
    /// </summary>
    /// <typeparam name="TModel">Тип модели представления</typeparam>
    /// <param name="fieldDiscription">Описание к текушему полю</param>
    /// <param name="fieldDisplatyName">Отображаемое название поля</param>
    /// <param name="fieldName">Имя текущего свойства модели</param>
    /// <param name="formStyle">Стиль формы</param>
    /// <param name="value">Значение для поля</param>
    /// <param name="additionalAttributes">Дополнительные аттрибуты к input</param>
    /// <returns></returns>
    public static MvcHtmlString ApplicationTextBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, string fieldName, string fieldDisplatyName, string fieldDiscription = null, string formStyle = "blue", string value = null, IDictionary<string, object> additionalAttributes = null)
    {
      string inputBlockTag_InnerHtml = String.Empty;

      if (additionalAttributes == null)
        additionalAttributes = new Dictionary<string, object>();

      Dictionary<string, object> additionalParams = new Dictionary<string, object>(additionalAttributes)
      {
        {"placeholder", fieldDisplatyName},
        {"class", String.Format("b-ib__input b-ib__input_type_{0}", formStyle)}
      };

      string textBoxForAsString = htmlHelper.TextBox(fieldName, value, additionalParams).ToString();

      return RenderApplicationTextBoxFor(inputBlockTag_InnerHtml, textBoxForAsString, fieldDisplatyName, fieldDiscription, formStyle, IsNeedBtnClear(additionalParams));
    }
  }
}