namespace Southworks.IdentityModel.MultiProtocolIssuer.Configuration
{
    using System;
    using System.Configuration;
    using Southworks.IdentityModel.MultiProtocolIssuer.Model;
    using Southworks.IdentityModel.MultiProtocolIssuer.Utilities;

    public class DefaultConfigurationRepository : IConfigurationRepository
    {
        public ClaimProvider RetrieveIssuer(Uri identifier)
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;
            var claimProvider = configuration.ClaimProviders[identifier.ToString()];

            var issuer = claimProvider.ToModel();
            return issuer;
        }

        public MultiProtocolIssuer RetrieveMultiProtocolIssuer()
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;

            return new MultiProtocolIssuer
            {
                Identifier = new Uri(configuration.Identifier),
                ReplyUrl = new Uri(configuration.ResponseEndpoint),
                SigningCertificate =
                    CertificateUtil.GetCertificate(
                        configuration.SigningCertificate.StoreName,
                        configuration.SigningCertificate.StoreLocation,
                        configuration.SigningCertificate.FindValue)
            };
        }

        public Scope RetrieveScope(Uri identifier)
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;

            var scope = configuration.Scopes[identifier.ToString()];
            var model = scope.ToModel();

            return model;
        }
    }
}
