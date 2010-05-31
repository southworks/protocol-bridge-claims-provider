namespace Southworks.IdentityModel.MultiProtocolIssuer.Tests.Configuration
{
    using System.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Southworks.IdentityModel.MultiProtocolIssuer.Configuration;

    [TestClass]
    public class MultiProtocolIssuerConfigurationFixture
    {
        [TestMethod]
        public void ShouldGetMultiProtocolIssuerattributes()
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;

            Assert.AreEqual("https://stsrealm", configuration.Identifier);
            Assert.AreEqual("https://response-endpoint", configuration.ResponseEndpoint);
        }

        [TestMethod]
        public void ShouldReadClaimProviders()
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;

            Assert.AreEqual(3, configuration.ClaimProviders.Count);

            Assert.AreEqual("provider0", configuration.ClaimProviders[0].Name);
            Assert.AreEqual("https://providerurl0", configuration.ClaimProviders[0].Uri);
            Assert.AreEqual("protocolHandler0", configuration.ClaimProviders[0].ProtocolHandler);
            Assert.AreEqual("profileA", configuration.ClaimProviders[0].Profile);
            Assert.AreEqual(true, configuration.ClaimProviders[0].AllowCompleteProfileForm);

            Assert.AreEqual("provider1", configuration.ClaimProviders[1].Name);
            Assert.AreEqual("https://providerurl1", configuration.ClaimProviders[1].Uri);
            Assert.AreEqual("protocolHandler1", configuration.ClaimProviders[1].ProtocolHandler);
            Assert.AreEqual("profileB", configuration.ClaimProviders[1].Profile);
            Assert.AreEqual(false, configuration.ClaimProviders[1].AllowCompleteProfileForm);
        }

        [TestMethod]
        public void ShouldReadScopes()
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;

            Assert.AreEqual(1, configuration.Scopes.Count);

            Assert.AreEqual("https://relyingpartyidentifier/", configuration.Scopes[0].Identifier);
            Assert.AreEqual("https://relyingpartyidentifier/theurl", configuration.Scopes[0].Uri);
        }

        [TestMethod]
        public void ShouldGetAScopebyItsUri()
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;
            Assert.IsNotNull(configuration.Scopes["https://relyingpartyidentifier/"]);
        }
        
        [TestMethod]
        public void ShouldGetAnIssuerbyItsName()
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;
            Assert.IsNotNull(configuration.ClaimProviders["provider0"]);
        }

        [TestMethod]
        public void ShouldReadClaimRequirementsInsideAGivenScope()
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;
            var claimRequirements = configuration.Scopes[0].ClaimRequirements;

            Assert.AreEqual(4, claimRequirements.Count);

            Assert.AreEqual("name", claimRequirements[0].Name);
            Assert.AreEqual("https://profile-A/name", claimRequirements[0].Type);
            Assert.AreEqual("Required", claimRequirements[0].DemandLevel);

            Assert.AreEqual("email", claimRequirements[1].Name);
            Assert.AreEqual("https://profile-A/email", claimRequirements[1].Type);
            Assert.AreEqual("Request", claimRequirements[1].DemandLevel);

            Assert.AreEqual("name", claimRequirements[2].Name);
            Assert.AreEqual("https://profile-B/name", claimRequirements[2].Type);
            Assert.AreEqual("Required", claimRequirements[2].DemandLevel);

            Assert.AreEqual("email", claimRequirements[3].Name);
            Assert.AreEqual("https://profile-B/email", claimRequirements[3].Type);
            Assert.AreEqual("Request", claimRequirements[3].DemandLevel);
        }

        [TestMethod]
        public void ShouldReadAllowedIssuersInsideAGivenScope()
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;
            var issuers = configuration.Scopes[0].Issuers;

            Assert.AreEqual(2, issuers.Count);

            Assert.AreEqual("name0", issuers[0].Name);
            Assert.AreEqual("name1", issuers[1].Name);
        }

        [TestMethod]
        public void ShouldReadClaimProviderParameters()
        {
            var configuration = ConfigurationManager.GetSection("southworks.identityModel/multiProtocolIssuer") as MultiProtocolIssuerSection;
            var claimProvider = configuration.ClaimProviders["provider2"];

            Assert.AreEqual("key1", claimProvider.Params[0].Key);
            Assert.AreEqual("value1", claimProvider.Params[0].Value);

            Assert.AreEqual("key2", claimProvider.Params[1].Key);
            Assert.AreEqual("value2", claimProvider.Params[1].Value);
        }
    }
}
