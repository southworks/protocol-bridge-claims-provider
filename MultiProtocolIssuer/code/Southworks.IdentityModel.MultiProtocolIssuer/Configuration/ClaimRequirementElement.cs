namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System.Configuration;

    public class ClaimRequirementElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("type", IsRequired = true, IsKey = true)]
        public string Type
        {
            get { return (string)this["type"]; }
        }

        [ConfigurationProperty("demandLevel", IsRequired = true)]
        public string DemandLevel
        {
            get { return (string)this["demandLevel"]; }
        }
    }
}