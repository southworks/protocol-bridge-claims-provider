namespace Southworks.IdentityModel.MultiProtocolIssuer.SecurityTokenService
{
    using System.Web;
    using Microsoft.IdentityModel.Configuration;
    using Microsoft.IdentityModel.SecurityTokenService;
    using Southworks.IdentityModel.MultiProtocolIssuer.Configuration;

    public class MultiProtocolSecurityTokenServiceConfiguration : SecurityTokenServiceConfiguration
    {
        private const string MultiProtocolSecurityTokenServiceConfigurationKey = "MultiProtocolSecurityTokenServiceConfigurationKey";
        private static readonly object syncRoot = new object();
        private readonly Southworks.IdentityModel.MultiProtocolIssuer.Model.MultiProtocolIssuer serviceProperties;

        public MultiProtocolSecurityTokenServiceConfiguration(IConfigurationRepository configurationRepository) : base()
        {
            this.serviceProperties = configurationRepository.RetrieveMultiProtocolIssuer();
        }                    

        public MultiProtocolSecurityTokenServiceConfiguration()
            : this(new DefaultConfigurationRepository())
        {
            this.SigningCredentials = new X509SigningCredentials(this.serviceProperties.SigningCertificate);
            this.TokenIssuerName = this.serviceProperties.Identifier.ToString();
            this.SecurityTokenService = typeof(MultiProtocolSecurityTokenService);
        }

        public static MultiProtocolSecurityTokenServiceConfiguration Current
        {
            get
            {
                HttpApplicationState httpAppState = HttpContext.Current.Application;

                MultiProtocolSecurityTokenServiceConfiguration customConfiguration = httpAppState.Get(MultiProtocolSecurityTokenServiceConfigurationKey) as MultiProtocolSecurityTokenServiceConfiguration;

                if (customConfiguration == null)
                {
                    lock (syncRoot)
                    {
                        customConfiguration = httpAppState.Get(MultiProtocolSecurityTokenServiceConfigurationKey) as MultiProtocolSecurityTokenServiceConfiguration;

                        if (customConfiguration == null)
                        {
                            customConfiguration = new MultiProtocolSecurityTokenServiceConfiguration();
                            httpAppState.Add(MultiProtocolSecurityTokenServiceConfigurationKey, customConfiguration);
                        }
                    }
                }

                return customConfiguration;
            }
        }
    }
}