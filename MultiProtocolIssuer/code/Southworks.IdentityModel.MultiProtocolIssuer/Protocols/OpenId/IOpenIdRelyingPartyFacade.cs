namespace Southworks.IdentityModel.MultiProtocolIssuer.Protocols.OpenID
{
    using System;

    using DotNetOpenAuth.OpenId;
    using DotNetOpenAuth.OpenId.RelyingParty;

    public interface IOpenIdRelyingPartyFacade
    {
        IAuthenticationRequest CreateRequest(Identifier userSuppliedIdentifier, Realm realm, Uri returnToUrl);

        IAuthenticationResponse GetResponse();
    }
}
