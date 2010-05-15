namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System.Configuration;

    public class ParameterCollection : ConfigurationElementCollection
    {
        public ParameterElement this[int index]
        {
            get { return (ParameterElement)BaseGet(index); }
        }

        public new ParameterElement this[string key]
        {
            get { return (ParameterElement)BaseGet(key); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ParameterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ParameterElement)element).Key;
        }
    }
}
