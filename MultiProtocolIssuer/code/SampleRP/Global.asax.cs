namespace SampleRP
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                string.Empty,
                new { controller = "Home", action = "UnSecure" });

            routes.MapRoute("Unsecure Page", "unsecure", new { controller = "Home", action = "UnSecure" });
            routes.MapRoute("Secure Page", "secure", new { controller = "Home", action = "Secure" });
            routes.MapRoute("Logout", "logout", new { controller = "Home", action = "LogOut" });
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}