namespace MultiProtocolIssuerSts.Controllers
{
    using System;
    using System.Web;

    public static class RequestUtilities
    {
        public static Uri GetRequestUrl(this HttpContextBase context)
        {
            var realHost = context.Request.Headers["HOST"];
            var requestUrl = context.Request.Url;
            string url = context.Request.Url.Scheme + "://" + realHost + context.Request.RawUrl;

            return new Uri(url);            
        }

        public static Uri GetRealAppRoot(this HttpContextBase context)
        {
            var realHost = context.Request.Headers["HOST"];

            var requestUrl = context.Request.Url;
            var appRoot = default(Uri);

            if (realHost.Contains(":"))
            {
                var realHostParts = realHost.Split(new[] { ':' });

                appRoot = new UriBuilder(requestUrl.Scheme, realHostParts[0], Convert.ToInt32(realHostParts[1]), context.Request.ApplicationPath).Uri;
            }
            else
            {
                appRoot = new UriBuilder(requestUrl.Scheme, realHost, requestUrl.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ? 80 : 443, context.Request.ApplicationPath).Uri;
            }

            return appRoot;
        }
    }
}