using System.Web.Mvc;

namespace MLMExchange.Areas.AdminPanel
{
  public class AdminPanelAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "AdminPanel";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "AdminPanel",
          "AdminPanel/{controller}/{action}/{id}",
          new { id = UrlParameter.Optional }
      );
    }
  }
}
