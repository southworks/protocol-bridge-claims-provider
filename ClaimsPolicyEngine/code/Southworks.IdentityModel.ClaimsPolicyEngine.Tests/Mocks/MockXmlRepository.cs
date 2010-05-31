namespace Southworks.IdentityModel.ClaimsPolicyEngine.Tests.Mocks
{
    using System.Xml;
    using System.Xml.Linq;

    public class MockXmlRepository : IXmlRepository
    {
        private XDocument returnedXDocument;

        public MockXmlRepository(string xmlDocumentPath)
        {
            using (XmlReader xmlReader = XmlReader.Create(xmlDocumentPath))
            {
                this.returnedXDocument = XDocument.Load(xmlReader);
            }
        }

        public XDocument Load(string name)
        {
            return this.returnedXDocument;
        }

        public void Save(string name, XDocument document)
        {
            this.returnedXDocument = document;
        }
    }
}
