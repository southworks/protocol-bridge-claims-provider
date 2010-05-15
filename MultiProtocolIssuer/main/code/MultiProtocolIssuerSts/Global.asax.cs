namespace MultiProtocolIssuerSts
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Process Request", string.Empty, new { controller = "Authentication", action = "ProcessFederationRequest" });
            routes.MapRoute("Home Realm Discovery", "hrd", new { controller = "Authentication", action = "HomeRealmDiscovery" });
            routes.MapRoute("Process Authentication", "authenticate", new { controller = "Authentication", action = "Authenticate" });
            routes.MapRoute("Process Authentication Response", "response", new { controller = "Authentication", action = "ProcessResponse" });
            routes.MapRoute(
                "FederationMetadata",
                "FederationMetadata/2007-06/FederationMetadata.xml",
                new { controller = "FederationMetadata", action = "FederationMetadata" });
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}