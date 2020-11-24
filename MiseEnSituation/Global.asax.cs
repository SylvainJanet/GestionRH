using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MiseEnSituation
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas(); // zones de routages
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters); // filtres
            RouteConfig.RegisterRoutes(RouteTable.Routes); // config des routes
            BundleConfig.RegisterBundles(BundleTable.Bundles); // config des bundles
        }
    }
}
