namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System.Configuration;
    using System.ServiceModel.Configuration;

    public class MultiProtocolIssuerSection : ConfigurationSection
    {
        [ConfigurationProperty("signingCertificate", IsRequired = true)]
        public X509DefaultServiceCertificateElement SigningCertificate
        {
            get { return (X509DefaultServiceCertificateElement)base["signingCertificate"]; }
        }

        [ConfigurationProperty("identifier", IsRequired = true)]
        public string Identifier
        {
            get { return (string)this["identifier"]; }
        }

        [ConfigurationProperty("responseEndpoint", IsRequired = true)]
        public string ResponseEndpoint
        {
            get { return (string)this["responseEndpoint"]; }
        }

        [ConfigurationProperty("claimProviders", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ClaimProviderCollection))]
        public ClaimProviderCollection ClaimProviders
        {
            get { return (ClaimProviderCollection)base["claimProviders"]; }
        }

        [ConfigurationProperty("scopes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ScopeCollection), AddItemName = "scope")]
        public ScopeCollection Scopes
        {
            get { return (ScopeCollection)base["scopes"]; }
        }
    }
}