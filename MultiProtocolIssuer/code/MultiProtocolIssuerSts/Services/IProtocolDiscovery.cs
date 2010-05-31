namespace MultiProtocolIssuerSts.Services
{
    using Southworks.IdentityModel.MultiProtocolIssuer.Model;
    using Southworks.IdentityModel.MultiProtocolIssuer.Protocols;

    public interface IProtocolDiscovery
    {
        IProtocolHandler RetrieveProtocolHandler(ClaimProvider issuer);
    }
}