namespace Southworks.IdentityModel.MultiProtocolIssuer.Protocols
{
    using System;
    using System.Web;
    using Microsoft.IdentityModel.Claims;
    using Southworks.IdentityModel.MultiProtocolIssuer.Model;

    public interface IProtocolHandler
    {
        void ProcessSignInRequest(Scope scope, HttpContextBase httpContext);

        IClaimsIdentity ProcessSignInResponse(string realm, string originalUrl, HttpContextBase httpContext);
    }
}