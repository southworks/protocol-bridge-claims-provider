namespace Southworks.IdentityModel.ClaimsPolicyEngine.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Southworks.IdentityModel.ClaimsPolicyEngine;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Model;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Tests.Mocks;

    [TestClass]
    public class IntegrationFixture
    {
        private static ServiceHost host;

        [TestInitialize]
        public void InitializaFixture()
        {
            var store = new XmlPolicyStore("My Xml Store Path", new MockXmlRepository(@"content\integrationTest3.xml"));
            host = new ServiceHost(store, new[] { new Uri("http://localhost:3333") });
            host.AddServiceEndpoint(typeof(IPolicyStore), new BasicHttpBinding(), "policystore");
            host.Open();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (host != null && host.State.Equals(CommunicationState.Opened))
                host.Close();
        }

        [TestMethod]
        [DeploymentItem(@"content\integrationTest1.xml", "content")]
        public void ShouldPassEvaluateRuleIfFixedOutputValue()
        {
            var store = new XmlPolicyStore("My Xml Store Path", new MockXmlRepository(@"content\integrationTest1.xml"));
            ClaimsPolicyEvaluator evaluator = new ClaimsPolicyEvaluator(store);
            Claim inputClaim = new Claim("http://myInputClaimType1", "myInputClaim", string.Empty, "http://myIssuer1");
            IEnumerable<Claim> evaluatedOutputClaims = evaluator.Evaluate(new Uri("http://localhost/1"), new[] { inputClaim });

            Assert.IsNotNull(evaluatedOutputClaims);
            Assert.AreEqual(1, evaluatedOutputClaims.Count());
            Assert.AreEqual("http://myOutputClaimType1", evaluatedOutputClaims.ElementAt(0).ClaimType);
            Assert.AreEqual("myOutputClaimValue", evaluatedOutputClaims.ElementAt(0).Value);
        }

        [TestMethod]
        [DeploymentItem(@"content\integrationTest2.xml", "content")]
        public void ShouldPassEvaluateRuleIfCopyOutputValueFromInputValue()
        {
            var store = new XmlPolicyStore("My Xml Store Path", new MockXmlRepository(@"content\integrationTest2.xml"));

            PolicyScope scope = store.RetrieveScope(new Uri("http://localhost/1"));
            var issuer = scope.Issuers.ElementAt(0);
            IList<InputPolicyClaim> inputClaims = new List<InputPolicyClaim>();
            ClaimType claimType = new ClaimType("http://myClaimType", "myClaimType");
            inputClaims.Add(new InputPolicyClaim(issuer, claimType, "*"));
            PolicyRule newRule = new PolicyRule(AssertionsMatch.Any, inputClaims, new OutputPolicyClaim(claimType, string.Empty, CopyFromConstants.InputValue));

            store.AddPolicyRule(new Uri("http://localhost/1"), newRule);
            string claimValue = "myInputClaimValue33";

            ClaimsPolicyEvaluator evaluator = new ClaimsPolicyEvaluator(store);
            Claim inputClaim = new Claim("http://myClaimType", claimValue, string.Empty, "http://myIssuer1");
            IEnumerable<Claim> evaluatedOutputClaims = evaluator.Evaluate(new Uri("http://localhost/1"), new[] { inputClaim });

            Assert.IsNotNull(evaluatedOutputClaims);
            Assert.AreEqual(1, evaluatedOutputClaims.Count());
            Assert.AreEqual("http://myClaimType", evaluatedOutputClaims.ElementAt(0).ClaimType);
            Assert.AreEqual(claimValue, evaluatedOutputClaims.ElementAt(0).Value);
        }

        [TestMethod]
        [DeploymentItem(@"content\integrationTest2.xml", "content")]
        public void ShouldPassEvaluateRuleIfCopyOutputValueFromInputIssuer()
        {
            var store = new XmlPolicyStore("My Xml Store Path", new MockXmlRepository(@"content\integrationTest2.xml"));

            PolicyScope scope = store.RetrieveScope(new Uri("http://localhost/1"));
            var issuer = scope.Issuers.ElementAt(0);
            IList<InputPolicyClaim> inputClaims = new List<InputPolicyClaim>();
            ClaimType claimType = new ClaimType("http://myClaimType", "myClaimType");
            inputClaims.Add(new InputPolicyClaim(issuer, claimType, "*"));
            PolicyRule newRule = new PolicyRule(AssertionsMatch.Any, inputClaims, new OutputPolicyClaim(claimType, string.Empty, CopyFromConstants.InputIssuer));

            store.AddPolicyRule(new Uri("http://localhost/1"), newRule);
            string claimValue = "myInputClaimValue33";

            ClaimsPolicyEvaluator evaluator = new ClaimsPolicyEvaluator(store);
            Claim inputClaim = new Claim("http://myClaimType", claimValue, string.Empty, "http://myIssuer1");
            IEnumerable<Claim> evaluatedOutputClaims = evaluator.Evaluate(new Uri("http://localhost/1"), new[] { inputClaim });

            Assert.IsNotNull(evaluatedOutputClaims);
            Assert.AreEqual("http://myClaimType", evaluatedOutputClaims.ElementAt(0).ClaimType);
        }

        [TestMethod]
        [DeploymentItem(@"content\integrationTest3.xml", "content")]
        public void ShouldRetriveScopeViaWCF()
        {
            ChannelFactory<IPolicyStore> factory = new ChannelFactory<IPolicyStore>(new BasicHttpBinding(), new EndpointAddress("http://localhost:3333/policystore"));
            IPolicyStore store = factory.CreateChannel();
            var scopes = store.RetrieveScopes();
            Assert.AreEqual(1, scopes.Count());
            Assert.AreEqual(2, scopes.ElementAt(0).Rules.Count());
        }

        [TestMethod]
        [DeploymentItem(@"content\integrationTest3.xml", "content")]
        public void ShouldAddRuleViaWCF()
        {
            ChannelFactory<IPolicyStore> factory = new ChannelFactory<IPolicyStore>(new BasicHttpBinding(), new EndpointAddress("http://localhost:3333/policystore"));
            IPolicyStore store = factory.CreateChannel();
            var scope = store.RetrieveScopes().ElementAt(0);
            
            InputPolicyClaim inputClaim = new InputPolicyClaim(scope.Issuers.ElementAt(0), scope.ClaimTypes.ElementAt(0), "thevalue");
            OutputPolicyClaim outputClaim = new OutputPolicyClaim(scope.ClaimTypes.ElementAt(0), CopyFromConstants.InputValue);

            PolicyRule rule = new PolicyRule(AssertionsMatch.All, new[] { inputClaim }, outputClaim);

            store.AddPolicyRule(scope.Uri, rule);
            
            var updatedScope = store.RetrieveScopes().ElementAt(0);

            Assert.AreEqual(3, updatedScope.Rules.Count());
        }
    }
}
