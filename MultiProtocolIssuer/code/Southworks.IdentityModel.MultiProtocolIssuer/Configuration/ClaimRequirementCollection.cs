namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System.Configuration;

    public class ClaimRequirementCollection : ConfigurationElementCollection
    {
        public ClaimRequirementElement this[int index]
        {
            get { return (ClaimRequirementElement)BaseGet(index); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ClaimRequirementElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClaimRequirementElement)element).Type;
        }
    }
}