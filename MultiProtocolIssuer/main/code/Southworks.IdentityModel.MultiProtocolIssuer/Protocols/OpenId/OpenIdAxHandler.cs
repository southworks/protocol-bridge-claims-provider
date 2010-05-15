namespace Southworks.IdentityModel.MultiProtocolIssuer.Protocols.OpenID
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
    using DotNetOpenAuth.OpenId.RelyingParty;

    using Microsoft.IdentityModel.Claims;

    using Southworks.IdentityModel.MultiProtocolIssuer.Model;

    public class OpenIdAxHandler : ProtocolHandlerBase
    {
        private readonly IOpenIdRelyingPartyFacade openIdRelyingParty;

        public OpenIdAxHandler(ClaimProvider issuer)
            : this(issuer, new OpenIdRelyingPartyFacade())
        {
        }

        public OpenIdAxHandler(ClaimProvider issuer, IOpenIdRelyingPartyFacade openIdRelyingParty) : base(issuer)
        {
            if (openIdRelyingParty == null)
                throw new ArgumentNullException("openIdRelyingParty");

            this.openIdRelyingParty = openIdRelyingParty;
        }

        public override void ProcessSignInRequest(Scope scope, HttpContextBase httpContext)
        {
            var request = this.openIdRelyingParty.CreateRequest(this.Issuer.Url, this.MultiProtocolIssuer.Identifier, this.MultiProtocolIssuer.ReplyUrl);
            var ax = new FetchRequest();
            
            foreach (var requirement in scope.ClaimTypeRequirements)
            {
                ax.Attributes.Add(new AttributeRequest(requirement.ClaimType, requirement.DemandLevel != ClaimDemandLevel.NoRequest));
            }

            request.AddExtension(ax);
            request.RedirectToProvider();
        }

        public override IClaimsIdentity ProcessSignInResponse(string realm, string originalUrl, HttpContextBase httpContext)
        {
            var response = this.openIdRelyingParty.GetResponse();

            if (response != null)
            {
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        var ax = response.GetExtension<FetchResponse>();
                        
                        // TODO: this is not intuitive for the protocol handler implementer
                        var scope = this.Configuration.RetrieveScope(new Uri(realm));
                        
                        var claims = new List<Claim>();
                        foreach (var requirement in scope.ClaimTypeRequirements)
                        {
                            if (ax.Attributes.Contains(requirement.ClaimType))
                            {
                                var attribute = ax.Attributes[requirement.ClaimType];
                                claims.Add(new Claim(attribute.TypeUri, attribute.Values.First()));
                            }
                        }

                        claims.Add(new Claim(ClaimTypes.Name, response.ClaimedIdentifier));

                        return new ClaimsIdentity(claims, "OpenID");

                    case AuthenticationStatus.Canceled:
                        break;
                    case AuthenticationStatus.Failed:
                        break;
                }
            }

            return null;
        }
    }
}