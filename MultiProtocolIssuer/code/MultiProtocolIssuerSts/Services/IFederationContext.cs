namespace MultiProtocolIssuerSts.Services
{
    public interface IFederationContext
    {
        string Realm { get; set; }

        string OriginalUrl { get; set; }

        string IssuerName { get; set; }

        string GetValue(string key);

        void SetValue(string key, string value);
    }
}