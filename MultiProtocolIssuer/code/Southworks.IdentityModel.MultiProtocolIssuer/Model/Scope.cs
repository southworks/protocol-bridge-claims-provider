namespace Southworks.IdentityModel.MultiProtocolIssuer.Model
{
    using System;
    using System.Collections.Generic;

    public class Scope
    {
        public Uri Identifier { get; set; }
        
        public Uri Url { get; set; }

        public IEnumerable<ClaimTypeRequirement> ClaimTypeRequirements { get; set; }

        public IEnumerable<ClaimProvider> AllowedIssuers { get; set; }

        public bool UseClaimsPolicyEngine { get; set; }
    }
}
