using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLMExchange.Lib
{
  public class GetViewStyle
  {
    protected const string _PrivateStyleDir = "Content\\stylesheets\\pages\\";
    protected ViewContext _ViewContext;

    public GetViewStyle(ViewContext viewContext)
    {
      _ViewContext = viewContext;
    }

    public MvcHtmlString RenderPrivateStyle()
    {
      string pageName = _ViewContext.RouteData.Values["controller"] + "__" + _ViewContext.RouteData.Values["action"];
      string fullStylePath = HttpContext.Current.Server.MapPath(@"~/" + _PrivateStyleDir + pageName + ".css");
      string stylePath = "\\" + _PrivateStyleDir + pageName + ".css";

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