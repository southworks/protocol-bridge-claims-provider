namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System.Configuration;

    public class ScopeElement : ConfigurationElement
    {
        [ConfigurationProperty("identifier", IsRequired = true)]
        public string Identifier
        {
            get { return (string)this["identifier"]; }
        }

        [ConfigurationProperty("uri", IsRequired = true, IsKey = true)]
        public string Uri
        {
            get { return (string)this["uri"]; }
        }

        [ConfigurationProperty("useClaimsPolicyEngine", IsRequired = false, DefaultValue = false)]
        public bool UseClaimsPolicyEngine
        {
            get { return (bool)this["useClaimsPolicyEngine"]; }
        }

        [ConfigurationProperty("claimRequirements", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ClaimRequirementCollection))]
        public ClaimRequirementCollection ClaimRequirements
        {
            get { return (ClaimRequirementCollection)base["claimRequirements"]; }
        }

        [ConfigurationProperty("allowedClaimProviders", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(AllowedClaimProviderCollection))]
        public AllowedClaimProviderCollection Issuers
        {
            get { return (AllowedClaimProviderCollection)base["allowedClaimProviders"]; }
        }
    }
}