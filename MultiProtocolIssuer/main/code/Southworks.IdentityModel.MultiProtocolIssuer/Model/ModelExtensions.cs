namespace Southworks.IdentityModel.MultiProtocolIssuer.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;

    using Southworks.IdentityModel.MultiProtocolIssuer.Configuration;

    public static class ModelExtensions
    {
        public static DemandLevel FromModel(this ClaimDemandLevel level)
        {
            switch (level)
            {
                case ClaimDemandLevel.NoRequest:
                    return DemandLevel.NoRequest;
                case ClaimDemandLevel.Request:
                    return DemandLevel.Request;
                case ClaimDemandLevel.Require:
                    return DemandLevel.Require;
            }

            return DemandLevel.NoRequest;
        }

        public static ClaimProvider ToModel(this ClaimProviderElement claimProvider)
        {
            if (claimProvider == null)
                return null;

            return new ClaimProvider
            {
                Identifier = new Uri(claimProvider.Name),
                Url = new Uri(claimProvider.Uri),
                Protocol = claimProvider.ProtocolHandler,
                Profile = claimProvider.Profile,
                Parameters = claimProvider.Params.ToModel()
            };
        }

        public static NameValueCollection ToModel(this ParameterCollection parameters)
        {
            if (parameters == null)
                return null;

            var collection = new NameValueCollection();
            foreach (ParameterElement param in parameters)
            {
                collection.Add(param.Key, param.Value);
            }

            return collection;
        }

        public static Scope ToModel(this ScopeElement scopeElement)
        {
            if (scopeElement == null)
                return null;

            var scope = new Scope
            {
                Identifier = new Uri(scopeElement.Identifier),
                Url = new Uri(scopeElement.Uri),
                UseClaimsPolicyEngine = scopeElement.UseClaimsPolicyEngine
            };

            if (scopeElement.Issuers != null)
            {
                var issuers = new List<ClaimProvider>();
                foreach (AllowedClaimProviderElement allowedIssuer in scopeElement.Issuers)
                {
                    issuers.Add(allowedIssuer.ToModel());
                }

                scope.AllowedIssuers = issuers;
            }

            if (scopeElement.ClaimRequirements != null)
            {
                var claimRequirements = new List<ClaimTypeRequirement>();
                foreach (ClaimRequirementElement claimReq in scopeElement.ClaimRequirements)
                {
                    claimRequirements.Add(claimReq.ToModel());
                }

                scope.ClaimTypeRequirements = claimRequirements;
            }

            return scope;
        }

        public static ClaimProvider ToModel(this AllowedClaimProviderElement allowedIssuer)
        {
            return new ClaimProvider
            {
                Identifier = new Uri(allowedIssuer.Name)
            };          
        }

        public static ClaimTypeRequirement ToModel(this ClaimRequirementElement claimRequirement)
        {
            return new ClaimTypeRequirement
            {
                ClaimType = claimRequirement.Type,
                DemandLevel = (ClaimDemandLevel)Enum.Parse(typeof(ClaimDemandLevel), claimRequirement.DemandLevel)
            };          
        }
    }
}