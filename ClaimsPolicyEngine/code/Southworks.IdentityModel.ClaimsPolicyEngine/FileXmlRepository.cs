namespace Southworks.IdentityModel.ClaimsPolicyEngine
{
    using System.Xml;
    using System.Xml.Linq;

    public class FileXmlRepository : IXmlRepository
    {
        public XDocument Load(string name)
        {
            XDocument document = null;
            using (XmlReader xmlReader = XmlReader.Create(name))
            {
                document = XDocument.Load(xmlReader);
            }

            return document;
        }

        public void Save(string name, XDocument document)
        {
            document.Save(name);
        }
    }
}
