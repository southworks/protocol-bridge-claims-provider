namespace MultiProtocolIssuerSts.Services
{
    using System;
    using System.Configuration;

    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;

    using Southworks.IdentityModel.MultiProtocolIssuer.Model;
    using Southworks.IdentityModel.MultiProtocolIssuer.Protocols;

    public class DefaultProtocolDiscovery : IProtocolDiscovery
    {
        private readonly IUnityContainer container;

        public DefaultProtocolDiscovery()
        {
            var unitySection = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;

            if (unitySection == null)
            {
                throw new ArgumentException("unitySection");
            }

            this.container = new UnityContainer();
            
            unitySection.Configure(this.container);
        }

        public IProtocolHandler RetrieveProtocolHandler(ClaimProvider issuer)
        {            
            return this.container.Resolve<IProtocolHandler>(
                              issuer.Protocol,
                              new ParameterOverride("issuer", issuer));
        }
    }
}
