namespace MultiProtocolIssuerSts.Controllers
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.Web.Mvc;

    using Microsoft.IdentityModel.Claims;
    using Microsoft.IdentityModel.Protocols.WSFederation;
    using Microsoft.IdentityModel.Protocols.WSFederation.Metadata;
    using Microsoft.IdentityModel.Protocols.WSIdentity;
    using Microsoft.IdentityModel.SecurityTokenService;

    using Southworks.IdentityModel.MultiProtocolIssuer.Configuration;

    public class FederationMetadataController : Controller
    {
        private readonly IConfigurationRepository configuration;

        public FederationMetadataController()
            : this(new DefaultConfigurationRepository())
        {
        }

        public FederationMetadataController(IConfigurationRepository configuration)
        {
            this.configuration = configuration;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult FederationMetadata(string organizationAlias)
        {
            var appRoot = this.HttpContext.GetRealAppRoot();

            var federationMetadataUrl = new Uri(appRoot, this.Url.RouteUrl("FederationMetadata"));
            var signInUrl = new Uri(appRoot, this.Url.RouteUrl("Process Request"));

            var serviceProperties = this.configuration.RetrieveMultiProtocolIssuer();

            return this.File(this.GetFederationMetadata(signInUrl, serviceProperties.Identifier, serviceProperties.SigningCertificate), "text/xml");
        }

        public byte[] GetFederationMetadata(Uri passiveSignInUrl, Uri identifier, X509Certificate2 signingCertificate)
        {
            var credentials = new X509SigningCredentials(signingCertificate);

            // Figure out the hostname exposed from Azure and what port the service is listening on
            EndpointAddress realm = new EndpointAddress(identifier);
            EndpointAddress passiveEndpoint = new EndpointAddress(passiveSignInUrl);

            // Create metadata document for relying party
            EntityDescriptor entity = new EntityDescriptor(new EntityId(realm.Uri.AbsoluteUri));
            SecurityTokenServiceDescriptor sts = new SecurityTokenServiceDescriptor();
            entity.RoleDescriptors.Add(sts);

            // Add STS's signing key
            KeyDescriptor signingKey = new KeyDescriptor(credentials.SigningKeyIdentifier);
            signingKey.Use = KeyType.Signing;
            sts.Keys.Add(signingKey);

            // Add offered claim types
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.AuthenticationMethod));
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.AuthenticationInstant));
            sts.ClaimTypesOffered.Add(new DisplayClaim(ClaimTypes.Name));

            // Add passive federation endpoint
            sts.PassiveRequestorEndpoints.Add(passiveEndpoint);

            // Add supported protocols
            sts.ProtocolsSupported.Add(new Uri(WSFederationConstants.Namespace));

            // Add passive STS endpoint
            sts.SecurityTokenServiceEndpoints.Add(passiveEndpoint);

            // Set credentials with which to sign the metadata
            entity.SigningCredentials = credentials;

            // Serialize the metadata and convert it to an XElement
            MetadataSerializer serializer = new MetadataSerializer();
            MemoryStream stream = new MemoryStream();
            serializer.WriteMetadata(stream, entity);
            stream.Flush();

            return stream.ToArray();
        }
    }
}
