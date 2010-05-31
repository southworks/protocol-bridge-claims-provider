namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System.Configuration;

    public class ParameterElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true, IsKey = true)]
        public string Key
        {
            get { return (string)this["key"]; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
        }
    }
}