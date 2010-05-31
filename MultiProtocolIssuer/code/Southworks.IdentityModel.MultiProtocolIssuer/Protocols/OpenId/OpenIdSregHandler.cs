namespace Southworks.IdentityModel.MultiProtocolIssuer.Protocols.OpenID
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
    using DotNetOpenAuth.OpenId.RelyingParty;

    using Microsoft.IdentityModel.Claims;

    using Southworks.IdentityModel.MultiProtocolIssuer.Model;

    public class OpenIdSregHandler : ProtocolHandlerBase
    {
        private readonly IOpenIdRelyingPartyFacade openIdRelyingParty;

        public OpenIdSregHandler(ClaimProvider issuer)
            : this(issuer, new OpenIdRelyingPartyFacade())
        {
        }

        public OpenIdSregHandler(ClaimProvider issuer, IOpenIdRelyingPartyFacade openIdRelyingParty)
            : base(issuer)
        {
            if (openIdRelyingParty == null)
                throw new ArgumentNullException("openIdRelyingParty");

            this.openIdRelyingParty = openIdRelyingParty;
        }

        public override void ProcessSignInRequest(Scope scope, HttpContextBase httpContext)
        {
            var request = this.openIdRelyingParty.CreateRequest(this.Issuer.Url, this.MultiProtocolIssuer.Identifier, this.MultiProtocolIssuer.ReplyUrl);
            
            var sreg = new ClaimsRequest();

            foreach (var requirement in scope.ClaimTypeRequirements)
            {
                switch (requirement.ClaimType)
                {
                    case "http://schema.openid.net/namePerson/friendly":
                        sreg.Nickname = requirement.DemandLevel.FromModel();
                        break;
                    case "http://schema.openid.net/namePerson":
                        sreg.FullName = requirement.DemandLevel.FromModel();
                        break;
                    case "http://schema.openid.net/contact/email":
                        sreg.Email = requirement.DemandLevel.FromModel();
                        break;
                    case "http://schema.openid.net/birthDate":
                        sreg.BirthDate = requirement.DemandLevel.FromModel();
                        break;
                    case "http://schema.openid.net/person/gender":
                        sreg.Gender = requirement.DemandLevel.FromModel();
                        break;
                    case "http://schema.openid.net/contact/postalCode/home":
                        sreg.PostalCode = requirement.DemandLevel.FromModel();
                        break;
                    case "http://schema.openid.net/contact/country/home":
                        sreg.Country = requirement.DemandLevel.FromModel();
                        break;
                    case "http://schema.openid.net/pref/language":
                        sreg.Language = requirement.DemandLevel.FromModel();
                        break;
                    case "http://schema.openid.net/pref/timezone":
                        sreg.TimeZone = requirement.DemandLevel.FromModel();
                        break;
                }
            }

            request.AddExtension(sreg);
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
                        var sreg = response.GetExtension<ClaimsResponse>();
                        
                        // TODO: this is not intuitive for the protocol handler implementer
                        var scope = this.Configuration.RetrieveScope(new Uri(realm));
                        
                        var issuerIdentifier = this.Issuer.Identifier.ToString();

                        var claims = new List<Claim>();

                        foreach (var requirement in scope.ClaimTypeRequirements)
                        {
                            switch (requirement.ClaimType)
                            {
                                case "http://schema.openid.net/namePerson/friendly":
                                    claims.Add(new Claim(requirement.ClaimType, response.FriendlyIdentifierForDisplay, response.FriendlyIdentifierForDisplay.GetType().ToString(), issuerIdentifier));
                                    break;
                                case "http://schema.openid.net/namePerson":
                                    claims.Add(new Claim(requirement.ClaimType, sreg.FullName, sreg.FullName.GetType().ToString(), issuerIdentifier));
                                    break;
                                case "http://schema.openid.net/contact/email":
                                    claims.Add(new Claim(requirement.ClaimType, sreg.Email, sreg.Email.GetType().ToString(), issuerIdentifier));
                                    break;
                                case "http://schema.openid.net/birthDate":
                                    if (sreg.BirthDate != null)
                                    {
                                        claims.Add(new Claim(requirement.ClaimType, (sreg.BirthDate.HasValue ? sreg.BirthDate.Value.ToString() : string.Empty), sreg.BirthDate.GetType().ToString(), issuerIdentifier));
                                    }

                                    break;
                                case "http://schema.openid.net/person/gender":
                                    if (sreg.Gender != null)
                                    {
                                        claims.Add(new Claim(requirement.ClaimType, (sreg.Gender.HasValue ? sreg.Gender.Value.ToString() : string.Empty)));
                                    }

                                    break;
                                case "http://schema.openid.net/contact/postalCode/home":
                                    claims.Add(new Claim(requirement.ClaimType, sreg.PostalCode));
                                    break;
                                case "http://schema.openid.net/contact/country/home":
                                    claims.Add(new Claim(requirement.ClaimType, sreg.Country));
                                    break;
                                case "http://schema.openid.net/pref/language":
                                    claims.Add(new Claim(requirement.ClaimType, sreg.Language));
                                    break;
                                case "http://schema.openid.net/pref/timezone":
                                    claims.Add(new Claim(requirement.ClaimType, sreg.TimeZone));
                                    break;
                                default:
                                    break;
                            }
                        }

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