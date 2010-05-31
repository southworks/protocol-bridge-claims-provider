namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System;
    using Southworks.IdentityModel.MultiProtocolIssuer.Model;

    public interface IConfigurationRepository
    {
        ClaimProvider RetrieveIssuer(Uri identifier);

        Scope RetrieveScope(Uri identifier);

        MultiProtocolIssuer RetrieveMultiProtocolIssuer();
    }
}
