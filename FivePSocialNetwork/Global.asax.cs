using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FivePSocialNetwork
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

            SqlDependency.Start(ConfigurationManager.ConnectionStrings["FivePSocialNetWork"].ConnectionString);
            Application["Online"] = 0;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            Application["Online"] = (int)Application["Online"] + 1;
            Application.UnLock();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["Online"] = (int)Application["Online"] - 1;
            Application.UnLock();
        }

        protected void Application_End()
        {
            SqlDependency.Stop(ConfigurationManager.ConnectionStrings["FivePSocialNetWork"].ConnectionString);
        }
    }
}
