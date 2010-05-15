namespace SampleRP.Library
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.IdentityModel.Protocols.WSFederation;
    using Microsoft.IdentityModel.Web;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthenticateAndAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        public string Roles { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsSecureConnection)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "https is required to browse the page: '{0}'.", filterContext.HttpContext.Request.Url.AbsoluteUri));
            }

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                AuthenticateUser(filterContext);
            }
        }

        private static void AuthenticateUser(AuthorizationContext context)
        {
            var returnUrl = GetReturnUrl(context.RequestContext);

            // user is not authenticated and it's entering for the first time
            var fam = FederatedAuthentication.WSFederationAuthenticationModule;
            var signIn = new SignInRequestMessage(new Uri(fam.Issuer), fam.Realm)
            {
                Context = returnUrl.ToString(),
                Reply = returnUrl.ToString()
            };

            context.Result = new RedirectResult(signIn.WriteQueryString());
        }

        private static Uri GetReturnUrl(RequestContext context)
        {
            var request = context.HttpContext.Request;
            var reqUrl = request.Url;
            var wreply = new StringBuilder();

            wreply.Append(reqUrl.Scheme);     // e.g. "http"
            wreply.Append("://");
            wreply.Append(request.Headers["Host"] ?? reqUrl.Authority);
            wreply.Append(request.RawUrl);

            if (!request.ApplicationPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                wreply.Append("/");
            }

            return new Uri(wreply.ToString());
        }
    }
}