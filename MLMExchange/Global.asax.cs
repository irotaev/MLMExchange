﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;

namespace MLMExchange
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801
  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      Logic.Lib.ApplicationUnityContainer.UnityContainer.RegisterType<Logic.INHibernateManager, Logic.NHibernateManager>();

      ModelBinders.Binders.Add(typeof(decimal?), new MLMExchange.Lib.ASP.NET.CustomBinders.DecimalModelBinder());
    }

    protected void Application_Error(object sender, EventArgs e)
    {
      Exception exception = Server.GetLastError();      

      Logic.Application.NLogLogger.Info("Заход в Application_Error");
      Logic.Application.NLogLogger.Fatal(exception);

      Server.ClearError();
      
      Response.Redirect("/ExceptionHandler/ApplicationException");
    }
  }
}