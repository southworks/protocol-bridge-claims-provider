namespace Southworks.IdentityModel.ClaimsPolicyEngine.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Exceptions;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Properties;

    [DataContract]
    public class PolicyScope
    {
        public PolicyScope(Uri uri)
        {
            this.Uri = uri;
            this.Rules = new List<PolicyRule>();
            this.ClaimTypes = new List<ClaimType>();
            this.Issuers = new List<Issuer>();
        }

        public PolicyScope(Uri uri, IEnumerable<PolicyRule> rules)
            : this(uri)
        {
            foreach (PolicyRule rule in rules)
            {
                this.Rules.Add(rule);
            }
        }

        [DataMember]
        public Uri Uri
        {
            get;
            private set;
        }

        [DataMember]
        public IList<PolicyRule> Rules
        {
            get;
            private set;
        }

        [DataMember]
        public IList<ClaimType> ClaimTypes
        {
            get;
            private set;
        }

        [DataMember]
        public IList<Issuer> Issuers
        {
            get;
            set;
        }

        public void AddRule(PolicyRule rule)
        {
            foreach (var claim in rule.InputClaims)
            {
                claim.ClaimType.DisplayName = this.GenerateClaimTypeDisplayName(claim.ClaimType);
                if (!this.ClaimTypes.Contains(claim.ClaimType))
                {
                    this.ClaimTypes.Add(claim.ClaimType);
                }

                if (!this.Issuers.Contains(claim.Issuer))
                {
                    if (claim.Issuer == null)
                    {
                        throw new ArgumentException(Resources.IssuerNotNull);
                    }

                    throw new PolicyScopeException(
                        String.Format(CultureInfo.CurrentCulture, Resources.IssuerNotDefined, claim.Issuer.DisplayName));
                }
            }

            rule.OutputClaim.ClaimType.DisplayName = this.GenerateClaimTypeDisplayName(rule.OutputClaim.ClaimType);
            if (!this.ClaimTypes.Contains(rule.OutputClaim.ClaimType))
            {
                this.ClaimTypes.Add(rule.OutputClaim.ClaimType);
            }

            if (!this.Rules.Contains(rule))
            {
                this.Rules.Add(rule);
            }
        }

        public void AddClaimType(ClaimType claimType)
        {
            claimType.DisplayName = this.GenerateClaimTypeDisplayName(claimType);

            this.ClaimTypes.Add(claimType);
        }

        public void AddIssuer(Issuer issuer)
        {
            if (!this.Issuers.Contains(issuer))
            {
                this.Issuers.Add(issuer);
            }
        }

        public void RemoveIssuer(Issuer issuer)
        {
            foreach (var rule in this.Rules)
            {
                foreach (var claim in rule.InputClaims)
                {
                    if (claim.Issuer.Equals(issuer))
                    {
                        throw new PolicyScopeException(Resources.IssuerNotDefined);
                    }
                }
            }

            this.Issuers.Remove(issuer);
        }

        public void RemoveRule(PolicyRule rule)
        {
            this.Rules.Remove(rule);
        }

        private string GenerateClaimTypeDisplayName(ClaimType claimType)
        {
            var existingClaimType = this.ClaimTypes.FirstOrDefault(ct => ct.Equals(claimType));

            if (existingClaimType != null)
            {
                return existingClaimType.DisplayName;
            }

            return this.BuildDisplayName(claimType);
        }

        private string BuildDisplayName(ClaimType claimType)
        {
            if (string.IsNullOrEmpty(claimType.DisplayName) || this.ClaimTypes.Any(ct => ct.DisplayName == claimType.DisplayName))
            {
                var unique = false;
                var auxFullName = claimType.FullName;
                var displayName = string.Empty;

                if (auxFullName.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    auxFullName = auxFullName.Remove(auxFullName.Length - 1);
                }

                while (auxFullName.Length > 0 && !unique)
                {
                    var slashIndex = auxFullName.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);

                    displayName += auxFullName.Substring(slashIndex + 1);
                    unique = !this.ClaimTypes.Any(ct => ct.DisplayName == displayName);
                    auxFullName = auxFullName.Remove(slashIndex);
                }

                return unique ? displayName : claimType.FullName;
            }

            return claimType.DisplayName;
        }
    }
}