namespace Southworks.IdentityModel.MultiProtocolIssuer.SecurityTokenService
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using Microsoft.IdentityModel.Claims;
    using Microsoft.IdentityModel.Configuration;
    using Microsoft.IdentityModel.Protocols.WSTrust;
    using Microsoft.IdentityModel.SecurityTokenService;

    using Southworks.IdentityModel.ClaimsPolicyEngine;
    using Southworks.IdentityModel.MultiProtocolIssuer.Configuration;

    public class MultiProtocolSecurityTokenService : SecurityTokenService
    {
        private readonly IConfigurationRepository multiProtocolConfiguration;
        
        private Model.Scope scopeModel;

        public MultiProtocolSecurityTokenService(SecurityTokenServiceConfiguration configuration)
            : this(configuration, new DefaultConfigurationRepository())
        {
        }

        public MultiProtocolSecurityTokenService(SecurityTokenServiceConfiguration configuration, IConfigurationRepository multiProtocolConfiguration)
            : base(configuration)
        {
            this.multiProtocolConfiguration = multiProtocolConfiguration;            
        }

        protected override Scope GetScope(IClaimsPrincipal principal, RequestSecurityToken request)
        {
            this.scopeModel = this.ValidateAppliesTo(request.AppliesTo);

            var scope = new Scope(request.AppliesTo.Uri.OriginalString, SecurityTokenServiceConfiguration.SigningCredentials);
            scope.TokenEncryptionRequired = false;
            scope.ReplyToAddress = string.IsNullOrEmpty(request.ReplyTo) ? scope.AppliesToAddress : request.ReplyTo;

            return scope;
        }

        protected override IClaimsIdentity GetOutputClaimsIdentity(IClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            if (null == principal)
            {
                throw new ArgumentNullException("principal");
            }

            var outputIdentity = new ClaimsIdentity();
            IEnumerable<Claim> outputClaims;

            if (this.scopeModel.UseClaimsPolicyEngine)
            {
                IClaimsPolicyEvaluator evaluator = new ClaimsPolicyEvaluator(PolicyStoreFactory.Instance);
                outputClaims = evaluator.Evaluate(new Uri(scope.AppliesToAddress), ((IClaimsIdentity)principal.Identity).Claims);
            }
            else
            {
                outputClaims = ((IClaimsIdentity)principal.Identity).Claims;
            }
           
            outputIdentity.Claims.AddRange(outputClaims);

            return outputIdentity;
        }

        private Model.Scope ValidateAppliesTo(EndpointAddress appliesTo)
        {
            if (appliesTo == null)
            {
                throw new ArgumentNullException("appliesTo");
            }

            var scope = this.multiProtocolConfiguration.RetrieveScope(appliesTo.Uri);
            if (scope == null)
            {
                throw new InvalidRequestException(String.Format("The relying party '{0}' was not found.", appliesTo.Uri.OriginalString));
            }

            return scope;
        }
    }
}