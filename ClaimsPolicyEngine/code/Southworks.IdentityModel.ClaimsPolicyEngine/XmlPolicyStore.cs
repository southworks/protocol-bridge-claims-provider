namespace Southworks.IdentityModel.ClaimsPolicyEngine
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using System.Xml.Linq;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Exceptions;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Model;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Properties;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true, AddressFilterMode = AddressFilterMode.Any)]
    public class XmlPolicyStore : IPolicyStore
    {
        private readonly string storeName;
        private readonly IXmlRepository repository;

        public XmlPolicyStore()
        {
        }

        public XmlPolicyStore(string storeName, IXmlRepository repository)
        {
            if (string.IsNullOrEmpty(storeName))
            {
                throw new ArgumentException(string.Empty, "storeName");
            }

            this.storeName = storeName;
            this.repository = repository;
        }

        public void AddPolicyRule(Uri scopeUri, PolicyRule rule)
        {
            var scopes = this.RetrieveScopes() as IList<PolicyScope>;
            var policyScope = (from s in scopes
                               where s.Uri.ToString() == scopeUri.ToString()
                               select s).FirstOrDefault();

            if (policyScope == null)
            {
                throw new PolicyScopeException(Resources.ScopeNotFound);
            }

            policyScope.AddRule(rule);

            this.SaveScopes(scopes);
        }

        public void RemovePolicyRule(Uri scopeUri, PolicyRule rule)
        {
            var scopes = this.RetrieveScopes() as IList<PolicyScope>;
            var policyScope = (from s in scopes
                               where s.Uri.ToString() == scopeUri.ToString()
                               select s).FirstOrDefault();

            if (policyScope == null)
            {
                throw new PolicyScopeException(Resources.ScopeNotFound);
            }

            if (rule != null)
            {
                policyScope.RemoveRule(rule);
            }

            this.SaveScopes(scopes);
        }

        public IEnumerable<PolicyScope> RetrieveScopes()
        {
            var scopes = new List<PolicyScope>();
            var document = this.repository.Load(this.storeName);
            var scopesElement = document.Root.Descendants("scope");

            foreach (var scopeElement in scopesElement)
            {
                var scope = RetrieveScope(scopeElement);

                if (scopes.Count(s => s.Uri == scope.Uri) > 0)
                {
                    throw new PolicyScopeException(string.Format(CultureInfo.CurrentUICulture, Resources.RepeatedScope, scope.Uri));
                }

                scopes.Add(scope);
            }

            return scopes;
        }

        public PolicyScope RetrieveScope(Uri scopeUri)
        {
            var document = this.repository.Load(this.storeName);
            var scopesElement = document.Root.Descendants("scope");

            foreach (var scopeElement in scopesElement)
            {
                var scope = RetrieveScope(scopeElement);

                if (scope.Uri == scopeUri)
                {
                    return scope;
                }
            }

            return null;
        }

        public void AddIssuer(Uri scopeUri, Issuer issuer)
        {
            IList<PolicyScope> scopes = this.RetrieveScopes() as IList<PolicyScope>;

            var policyScope = (from s in scopes
                               where s.Uri.ToString() == scopeUri.ToString()
                               select s).FirstOrDefault();

            if (policyScope == null)
            {
                throw new PolicyScopeException(Resources.ScopeNotFound);
            }

            policyScope.AddIssuer(issuer);

            this.SaveScopes(scopes);
        }

        public void RemoveIssuer(Uri scopeUri, Issuer issuer)
        {
            IList<PolicyScope> scopes = this.RetrieveScopes() as IList<PolicyScope>;

            var policyScope = (from s in scopes
                               where s.Uri.ToString() == scopeUri.ToString()
                               select s).FirstOrDefault();

            if (policyScope == null)
            {
                throw new PolicyScopeException(Resources.ScopeNotFound);
            }

            policyScope.RemoveIssuer(issuer);

            this.SaveScopes(scopes);
        }

        public Issuer RetrieveIssuer(Uri scopeUri, string issuerName)
        {
            var policyScope = this.RetrieveScope(scopeUri);
            if (policyScope == null)
            {
                throw new PolicyScopeException(Resources.ScopeNotFound);
            }

            issuerName = issuerName.ToUpperInvariant();
            return (from i in policyScope.Issuers
                          where i.DisplayName.ToUpperInvariant() == issuerName
                          select i).SingleOrDefault();
        }

        protected virtual void SaveScopes(IList<PolicyScope> policyScopes)
        {
            XDocument document = new XDocument();
            XElement scopesElement = new XElement("scopes");

            document.Add(scopesElement);
            foreach (var scope in policyScopes)
            {
                scopesElement.Add(SerializeScope(scope));
            }

            this.repository.Save(this.storeName, document);
        }

        private static XElement SerializeScope(PolicyScope scope)
        {
            XElement scopeElement = new XElement("scope");
            scopeElement.SetAttributeValue("uri", scope.Uri.ToString());

            XElement claimTypesElement = new XElement("claimTypes");
            scopeElement.Add(claimTypesElement);
            foreach (var claimType in scope.ClaimTypes)
            {
                claimTypesElement.Add(SerializaClaimType(claimType));
            }

            XElement issuersElement = new XElement("issuers");
            scopeElement.Add(issuersElement);
            foreach (var issuer in scope.Issuers)
            {
                issuersElement.Add(SerializaIssuer(issuer));
            }

            XElement rulesElement = new XElement("rules");
            scopeElement.Add(rulesElement);
            foreach (var rule in scope.Rules)
            {
                rulesElement.Add(SerializaRule(rule));
            }

            return scopeElement;
        }

        private static XElement SerializaIssuer(Issuer issuer)
        {
            XElement issuerElement = new XElement("issuer");
            issuerElement.SetAttributeValue("uri", issuer.Uri);
            issuerElement.SetAttributeValue("thumbprint", issuer.Thumbprint);
            issuerElement.SetAttributeValue("displayName", issuer.DisplayName);
            return issuerElement;
        }

        private static XElement SerializaClaimType(ClaimType claimType)
        {
            XElement claimTypeElement = new XElement("claimType");
            claimTypeElement.SetAttributeValue("fullName", claimType.FullName);
            claimTypeElement.SetAttributeValue("displayName", claimType.DisplayName);
            return claimTypeElement;
        }

        private static XElement SerializaRule(PolicyRule rule)
        {
            XElement ruleElement = new XElement("rule");
            ruleElement.SetAttributeValue("assertionsMatch", rule.AssertionsMatch.ToString());
            XElement inputElement = new XElement("input");
            ruleElement.Add(inputElement);
            foreach (var claim in rule.InputClaims)
            {
                inputElement.Add(SerializeInputClaim(claim));
            }

            ruleElement.Add(SerializeOutputClaim(rule.OutputClaim));
            return ruleElement;
        }

        private static XElement SerializeInputClaim(InputPolicyClaim claim)
        {
            XElement inputElement = new XElement("claim");
            inputElement.SetAttributeValue("type", claim.ClaimType.DisplayName);
            inputElement.SetAttributeValue("value", claim.Value);
            inputElement.SetAttributeValue("issuer", claim.Issuer.DisplayName);

            return inputElement;
        }

        private static XElement SerializeOutputClaim(OutputPolicyClaim outputPolicyClaim)
        {
            XElement outputElement = new XElement("output");
            outputElement.SetAttributeValue("type", outputPolicyClaim.ClaimType.DisplayName);

            if (!string.IsNullOrEmpty(outputPolicyClaim.Value))
            {
                outputElement.SetAttributeValue("value", outputPolicyClaim.Value);
            }

            if (outputPolicyClaim.CopyFromInput)
            {
                outputElement.SetAttributeValue("copyFrom", outputPolicyClaim.CopyFrom);
            }

            return outputElement;
        }

        private static PolicyScope RetrieveScope(XElement scopeElement)
        {
            IDictionary<string, string> claimTypes = RetrieveReferences(scopeElement.Element("claimTypes"), "claimType", "displayName", "fullName");

            IDictionary<string, Issuer> issuers = new Dictionary<string, Issuer>();
            PolicyScope scope = new PolicyScope(new Uri(scopeElement.Attribute("uri").Value), new List<PolicyRule>());

            var issuerElements = scopeElement.Element("issuers").Descendants("issuer");
            foreach (var item in issuerElements)
            {
                Issuer issuer = new Issuer(
                                    item.Attribute("uri").Value,
                                    item.Attribute("thumbprint").Value.ToUpperInvariant(),
                                    item.Attribute("displayName").Value);

                scope.AddIssuer(issuer);
                issuers.Add(issuer.DisplayName, issuer);
            }

            foreach (var item in claimTypes)
            {
                scope.AddClaimType(new ClaimType(item.Value, item.Key));
            }

            foreach (XElement ruleElement in scopeElement.Element("rules").Descendants("rule"))
            {
                AssertionsMatch assertionsMatch = RetrieveRuleAssertionsMatch(ruleElement);
                IEnumerable<InputPolicyClaim> inputClaims = RetrieveInputClaims(ruleElement, issuers, claimTypes);
                OutputPolicyClaim outputClaim = RetrieveOutputClaim(ruleElement, claimTypes);

                scope.AddRule(new PolicyRule(assertionsMatch, inputClaims, outputClaim));
            }

            return scope;
        }

        private static IDictionary<string, string> RetrieveReferences(XElement parentElement, string descendantsName, string keyAttribute, string valueAttribute)
        {
            IDictionary<string, string> claimTypes = new Dictionary<string, string>();
            foreach (XElement descendantElement in parentElement.Descendants(descendantsName))
            {
                string key = descendantElement.Attribute(keyAttribute).Value;
                string value = descendantElement.Attribute(valueAttribute).Value;
                claimTypes[key] = value;
            }

            return claimTypes;
        }

        private static AssertionsMatch RetrieveRuleAssertionsMatch(XElement ruleElement)
        {
            string assertionsMatchValue = ruleElement.Attribute("assertionsMatch").Value;
            switch (assertionsMatchValue.ToUpperInvariant())
            {
                case "ANY":
                    return AssertionsMatch.Any;
                case "ALL":
                    return AssertionsMatch.All;
                default:
                    return AssertionsMatch.NotSet;
            }
        }

        private static IEnumerable<InputPolicyClaim> RetrieveInputClaims(XElement ruleElement, IDictionary<string, Issuer> issuers, IDictionary<string, string> claimTypes)
        {
            List<InputPolicyClaim> inputClaims = new List<InputPolicyClaim>();
            foreach (XElement inputClaimElement in ruleElement.Element("input").Descendants("claim"))
            {
                string inputClaimValue = inputClaimElement.Attribute("value").Value;
                string inputIssuerDisplayName = inputClaimElement.Attribute("issuer").Value;
                string inputTypeDisplayName = inputClaimElement.Attribute("type").Value;

                if (!issuers.ContainsKey(inputIssuerDisplayName))
                {
                    throw new PolicyClaimException(string.Format(CultureInfo.CurrentUICulture, Resources.IssuerNotDefined, inputIssuerDisplayName));
                }

                if (!claimTypes.ContainsKey(inputTypeDisplayName))
                {
                    throw new PolicyClaimException(string.Format(CultureInfo.CurrentUICulture, Resources.ClaimTypeNotDefined, inputTypeDisplayName));
                }

                Issuer issuer = issuers[inputIssuerDisplayName];
                ClaimType claimType = new ClaimType(claimTypes[inputTypeDisplayName], inputTypeDisplayName);

                inputClaims.Add(new InputPolicyClaim(issuer, claimType, inputClaimValue));
            }

            return inputClaims;
        }

        private static OutputPolicyClaim RetrieveOutputClaim(XElement ruleElement, IDictionary<string, string> claimTypes)
        {
            string outputTypeDisplayName = ruleElement.Element("output").Attribute("type").Value;
            string outputClaimValue = string.Empty;
            if (ruleElement.Element("output").Attribute("value") != null)
            {
                outputClaimValue = ruleElement.Element("output").Attribute("value").Value;
            }

            string copyFrom = string.Empty;
            if (ruleElement.Element("output").Attribute("copyFrom") != null)
            {
                copyFrom = ruleElement.Element("output").Attribute("copyFrom").Value.ToUpperInvariant();
            }

            if (!claimTypes.ContainsKey(outputTypeDisplayName))
            {
                throw new PolicyClaimException(string.Format(CultureInfo.CurrentUICulture, Resources.ClaimTypeNotDefined, outputTypeDisplayName));
            }

            ClaimType claimType = new ClaimType(claimTypes[outputTypeDisplayName], outputTypeDisplayName);

            return new OutputPolicyClaim(claimType, outputClaimValue, copyFrom);
        }
    }
}