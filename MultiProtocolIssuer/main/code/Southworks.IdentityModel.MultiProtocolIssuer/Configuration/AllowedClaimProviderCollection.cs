namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System.Configuration;

    public class AllowedClaimProviderCollection : ConfigurationElementCollection
    {
        public AllowedClaimProviderElement this[int index]
        {
            get { return (AllowedClaimProviderElement)BaseGet(index); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AllowedClaimProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AllowedClaimProviderElement)element).Name;
        }
    }
}