using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Services.TempDbService;

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_OnEnd(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                var username = User.Identity.GetUserId();
                if (!string.IsNullOrWhiteSpace(username))
                {
                    ITempDbService dbService = new TempDbService();
                    dbService.RemoveUserLogin(username);
                }
            }
        }
    }
}
