namespace MultiProtocolIssuerSts.Controllers
{
    using System;
    using System.Globalization;
    using System.Security.Principal;
    using System.Web.Mvc;

    using Microsoft.IdentityModel.Claims;
    using Microsoft.IdentityModel.Protocols.WSFederation;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Web;

    using MultiProtocolIssuerSts.Services;

    using Southworks.IdentityModel.MultiProtocolIssuer.Configuration;
    using Southworks.IdentityModel.MultiProtocolIssuer.Model;
    using Southworks.IdentityModel.MultiProtocolIssuer.SecurityTokenService;

    [HandleError]
    public class AuthenticationController : Controller
    {
        private readonly IProtocolDiscovery protocolDiscovery;

        private readonly IFederationContext federationContext;

        private readonly IConfigurationRepository configuration;

        private readonly MultiProtocolIssuer multiProtocolServiceProperties;

        public AuthenticationController()
            : this(new DefaultProtocolDiscovery(), new FederationContext(), new DefaultConfigurationRepository())
        {
        }

        public AuthenticationController(IProtocolDiscovery defaultProtocolDiscovery, IFederationContext federationContext, IConfigurationRepository configuration)
        {
            this.protocolDiscovery = defaultProtocolDiscovery;
            this.federationContext = federationContext;
            this.configuration = configuration;
            this.multiProtocolServiceProperties = this.configuration.RetrieveMultiProtocolIssuer();
        }

        public ActionResult HomeRealmDiscovery()
        {
            return View("Authenticate");
        }
        
        public ActionResult Authenticate()
        {            
            var identifier = new Uri(this.Request.QueryString[WSFederationConstants.Parameters.HomeRealm]);

            ClaimProvider issuer = this.configuration.RetrieveIssuer(identifier);
            if (issuer == null)
            {
                return this.HomeRealmDiscovery();
            }

            var handler = this.protocolDiscovery.RetrieveProtocolHandler(issuer);
            if (handler == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The protocol handler '{0}' was not found in the container", issuer.Protocol));
            }

            this.federationContext.IssuerName = issuer.Identifier.ToString();
            var scope = this.configuration.RetrieveScope(new Uri(this.federationContext.Realm));
            if (scope == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The scope '{0}' was not found in the configuration", this.federationContext.Realm));
            }

            handler.ProcessSignInRequest(scope, this.HttpContext);
            
            return new EmptyResult();
        }

        public void ProcessResponse()
        {
            if (string.IsNullOrEmpty(this.federationContext.IssuerName) == null)
            {
                throw new InvalidOperationException("The context cookie was not found. Try to re-login");
            }

            var issuer = this.configuration.RetrieveIssuer(new Uri(this.federationContext.IssuerName));

            var handler = this.protocolDiscovery.RetrieveProtocolHandler(issuer);

            if (handler == null)
                throw new InvalidOperationException();

            IClaimsIdentity identity = handler.ProcessSignInResponse(
                                                                this.federationContext.Realm,
                                                                this.federationContext.OriginalUrl,
                                                                this.HttpContext);

            IClaimsIdentity outputIdentity = UpdateIssuer(identity, this.multiProtocolServiceProperties.Identifier.ToString(), issuer.Identifier.ToString());
            outputIdentity.Claims.Add(new Claim(ClaimTypes.AuthenticationMethod, issuer.Identifier.ToString(), ClaimValueTypes.String, this.multiProtocolServiceProperties.Identifier.ToString()));
            outputIdentity.Claims.Add(new Claim(ClaimTypes.AuthenticationInstant, DateTime.Now.ToString("o"), ClaimValueTypes.Datetime, this.multiProtocolServiceProperties.Identifier.ToString()));

            var sessionToken = new SessionSecurityToken(new ClaimsPrincipal(new IClaimsIdentity[] { outputIdentity }));
            FederatedAuthentication.WSFederationAuthenticationModule.SetPrincipalAndWriteSessionToken(sessionToken, true);

            Response.Redirect(this.federationContext.OriginalUrl, false);
            HttpContext.ApplicationInstance.CompleteRequest();
        }

        public ActionResult ProcessFederationRequest()
        {
            var action = Request.QueryString[WSFederationConstants.Parameters.Action];

            try
            {
                switch (action)
                {
                    case WSFederationConstants.Actions.SignIn:
                        {
                            var requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(Request.Url);
                            
                            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                            {
                                var sts = new MultiProtocolSecurityTokenService(MultiProtocolSecurityTokenServiceConfiguration.Current);
                                var responseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, User, sts);
                                responseMessage.Write(Response.Output);
                                Response.Flush();
                                Response.End();
                                HttpContext.ApplicationInstance.CompleteRequest();
                            }
                            else
                            {
                                // user not authenticated yet, look for whr, if not there go to HomeRealmDiscovery page
                                this.CreateFederationContext();

                                if (string.IsNullOrEmpty(this.Request.QueryString[WSFederationConstants.Parameters.HomeRealm]))
                                {
                                    return this.RedirectToAction("HomeRealmDiscovery");
                                }
                                else
                                {
                                    return this.Authenticate();
                                }
                            }
                        }

                        break;
                    case WSFederationConstants.Actions.SignOut:
                        {
                            var requestMessage = (SignOutRequestMessage)WSFederationMessage.CreateFromUri(Request.Url);
                            FederatedPassiveSecurityTokenServiceOperations.ProcessSignOutRequest(requestMessage, User, requestMessage.Reply, HttpContext.ApplicationInstance.Response);
                        }

                        break;
                    default:
                        throw new InvalidOperationException(
                            String.Format(
                                CultureInfo.InvariantCulture,
                                "The action '{0}' (Request.QueryString['{1}']) is unexpected. Expected actions are: '{2}' or '{3}'.",
                                String.IsNullOrEmpty(action) ? "<EMPTY>" : action,
                                WSFederationConstants.Parameters.Action,
                                WSFederationConstants.Actions.SignIn,
                                WSFederationConstants.Actions.SignOut));
                }
            }
            catch (Exception exception)
            {
                throw new Exception("An unexpected error occurred when processing the request. See inner exception for details.", exception);
            }

            return null;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
                throw new InvalidOperationException("Windows authentication is not supported.");
        }

        private static IClaimsIdentity UpdateIssuer(IClaimsIdentity input, string issuer, string originalIssuer)
        {
            IClaimsIdentity outputIdentity = new ClaimsIdentity();
            foreach (var claim in input.Claims)
            {
                outputIdentity.Claims.Add(new Claim(claim.ClaimType, claim.Value, claim.ValueType, issuer, originalIssuer));
            }

            return outputIdentity;
        }

        private void CreateFederationContext()
        {
            this.federationContext.OriginalUrl = this.HttpContext.GetRequestUrl().ToString();
            this.federationContext.Realm = Request.QueryString[WSFederationConstants.Parameters.Realm];
            this.federationContext.IssuerName = this.Request.QueryString[WSFederationConstants.Parameters.HomeRealm];
        }
    }
}
