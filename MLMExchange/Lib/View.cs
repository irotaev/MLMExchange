using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Lib
{
  /// <summary>
  /// Управление стилями представления
  /// </summary>
  public class ViewStyle
  {
    protected readonly string _PrivateStyleDir;
    protected ViewContext _ViewContext;

    public ViewStyle(ViewContext viewContext)
    {
      _PrivateStyleDir = Path.Combine("Content", "stylesheets", "pages");
      _ViewContext = viewContext;
    }

    /// <summary>
    /// Сгенерировать стиль для конкретной страницы
    /// </summary>
    /// <returns>Стиль</returns>
    public MvcHtmlString RenderPrivatePageStyle()
    {
      string pageName = String.Format("{0}__{1}", _ViewContext.RouteData.Values["controller"], _ViewContext.RouteData.Values["action"]);

      pageName = (_ViewContext.RouteData.DataTokens["area"] != null && !String.IsNullOrEmpty(_ViewContext.RouteData.DataTokens["area"].ToString())) 
        ? _ViewContext.RouteData.DataTokens["area"].ToString() + "_" + pageName : pageName;

      string fullStylePath = HttpContext.Current.Server.MapPath(@"~/" + Path.Combine(_PrivateStyleDir, pageName + ".css"));
      string stylePath = "\\" + Path.Combine(_PrivateStyleDir, pageName + ".css");

      if (!System.IO.File.Exists(fullStylePath))
        return null;

      TagBuilder styleLink = new TagBuilder("link");
      styleLink.MergeAttribute("media", "screen, projection");
      styleLink.MergeAttribute("rel", "stylesheet");
      styleLink.MergeAttribute("type", "text/css");
      styleLink.MergeAttribute("href", stylePath.Replace("\\", "/"));

      return MvcHtmlString.Create(styleLink.ToString(TagRenderMode.SelfClosing));
    }
  }
}