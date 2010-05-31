namespace MultiProtocolIssuerSts
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.UI;

    public class Default : Page
    {
        public void Page_Load(object sender, System.EventArgs e)
        {
            var originalPath = Request.Path;
            HttpContext.Current.RewritePath(Request.ApplicationPath, false);
            IHttpHandler httpHandler = new MvcHttpHandler();
            httpHandler.ProcessRequest(HttpContext.Current);
            HttpContext.Current.RewritePath(originalPath, false);
        }
    }
}
