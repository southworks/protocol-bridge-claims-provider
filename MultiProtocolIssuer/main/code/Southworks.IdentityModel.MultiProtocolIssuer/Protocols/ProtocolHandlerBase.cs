namespace Southworks.IdentityModel.MultiProtocolIssuer.Protocols
{
    using System;
    using System.Web;

    using Microsoft.IdentityModel.Claims;

    using Southworks.IdentityModel.MultiProtocolIssuer.Configuration;
    using Southworks.IdentityModel.MultiProtocolIssuer.Model;

    public abstract class ProtocolHandlerBase : IProtocolHandler
    {
        protected ProtocolHandlerBase(ClaimProvider issuer) : this(issuer, new DefaultConfigurationRepository())
        {
        }
        
        protected ProtocolHandlerBase(ClaimProvider issuer, IConfigurationRepository configuration)
        {
            if (issuer == null)
                throw new ArgumentNullException("issuer");

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            this.Issuer = issuer;
            this.Configuration = configuration;
            this.MultiProtocolIssuer = this.Configuration.RetrieveMultiProtocolIssuer();              
        }

        protected ClaimProvider Issuer { get; set; }

        protected IConfigurationRepository Configuration { get; set; }

        protected MultiProtocolIssuer MultiProtocolIssuer { get; set; }

        public abstract void ProcessSignInRequest(Scope scope, HttpContextBase httpContext);

        public abstract IClaimsIdentity ProcessSignInResponse(string realm, string originalUrl, HttpContextBase httpContext);        
    }
}