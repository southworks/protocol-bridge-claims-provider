namespace Southworks.IdentityModel.ClaimsPolicyEngine
{
    using System;
    using System.Collections.Generic;

    using Microsoft.IdentityModel.Claims;

    public interface IClaimsPolicyEvaluator
    {
        IEnumerable<Claim> Evaluate(Uri scope, IEnumerable<Claim> inputClaims);
    }
}