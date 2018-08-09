using System;
using System.Collections.Generic;
using System.IO;
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
            //RegisterRoutes(RouteTable.Routes);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
           
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
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


        //public static void RegisterRoutes(RouteCollection routes)
        //{
        //    routes.MapPageRoute("RouteError", "/Home/Exception");
        //}

        //protected void Application_Error()
        //{
        //    HttpContext httpContext = HttpContext.Current;
        //    if (httpContext != null)
        //    {
        //        RequestContext requestContext = ((MvcHandler)httpContext.CurrentHandler).RequestContext;
        //        /* When the request is ajax the system can automatically handle a mistake with a JSON response. 
        //           Then overwrites the default response */
        //        if (requestContext.HttpContext.Request.IsAjaxRequest())
        //        {
        //            httpContext.Response.Clear();
        //            string controllerName = requestContext.RouteData.GetRequiredString("controller");
        //            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
        //            IController controller = factory.CreateController(requestContext, controllerName);
        //            ControllerContext controllerContext = new ControllerContext(requestContext, (ControllerBase)controller);

        //            JsonResult jsonResult = new JsonResult
        //            {
        //                Data = new { success = false, serverError = "500" },
        //                JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //            };
        //            jsonResult.ExecuteResult(controllerContext);
        //            httpContext.Response.End();
        //        }
        //        else
        //        {
        //            //httpContext.Response.RedirectToRoute("/home/exception", httpContext.Error);
        //            HttpContext.Current.Response.RedirectToRoute("RouteError", new { Id = 404 });
        //        }
        //    }
        //}
    }
}
