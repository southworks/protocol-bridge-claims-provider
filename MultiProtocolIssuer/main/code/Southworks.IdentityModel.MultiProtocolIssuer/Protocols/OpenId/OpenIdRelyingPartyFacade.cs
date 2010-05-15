namespace Southworks.IdentityModel.MultiProtocolIssuer.Protocols.OpenID
{
    using System;

    using DotNetOpenAuth.OpenId;
    using DotNetOpenAuth.OpenId.RelyingParty;

    public class OpenIdRelyingPartyFacade : IOpenIdRelyingPartyFacade
    {
        private readonly OpenIdRelyingParty openIdRelyingParty = new OpenIdRelyingParty();

        public IAuthenticationRequest CreateRequest(Identifier userSuppliedIdentifier, Realm realm, Uri returnToUrl)
        {
            return this.openIdRelyingParty.CreateRequest(userSuppliedIdentifier, realm, returnToUrl);
        }

        public IAuthenticationResponse GetResponse()
        {
            return this.openIdRelyingParty.GetResponse();
        }
    }
}
