namespace Southworks.IdentityModel.ClaimsPolicyEngine
{
    using System.Xml.Linq;

    /// <summary>
    /// Repository that allows reading/writing an xml document to the storage medium.
    /// For local project this implies a File backed storage.
    /// For REST services, this implies creating a custom storage.
    /// </summary>
    public interface IXmlRepository
    {
        XDocument Load(string name);

        void Save(string name, XDocument document);
    }
}
