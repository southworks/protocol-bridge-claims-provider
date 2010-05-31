namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System.Configuration;

    public class ScopeCollection : ConfigurationElementCollection
    {
        public ScopeElement this[int index]
        {
            get { return (ScopeElement)BaseGet(index); }
        }
        
        public new ScopeElement this[string key]
        {
            get { return (ScopeElement)BaseGet(key); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ScopeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ScopeElement)element).Identifier;
        }
    }
}