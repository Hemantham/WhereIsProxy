using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WhereIsProxy
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

           // routes.MapRoute(
           //  name: "WhereIs",
           //  url: "v1/soap/ems",
           //  defaults: new { controller = "WhereIsMaps", action = "Map"}
           // );

           // routes.MapRoute(
           // name: "test",
           // url: "test",
           // defaults: new { controller = "WhereIsMaps", action = "Test" }
           //);


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

          
        }
    }
}