namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System.Configuration;

    public class ClaimProviderCollection : ConfigurationElementCollection
    {
        public ClaimProviderElement this[int index]
        {
            get { return (ClaimProviderElement)BaseGet(index); }
        }

        public new ClaimProviderElement this[string key]
        {
            get { return (ClaimProviderElement)BaseGet(key); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ClaimProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClaimProviderElement)element).Name;
        }
    }
}