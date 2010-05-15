namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System.Configuration;

    public class SouthworksIdentityModelSectionGroup : ConfigurationSectionGroup
    {
        [ConfigurationProperty("multiProtocolIssuer", IsRequired = true)]
        public MultiProtocolIssuerSection MultiProtocolIssuer
        {
            get { return (MultiProtocolIssuerSection)this.Sections["multiProtocolIssuer"]; }
        }
    }
}
