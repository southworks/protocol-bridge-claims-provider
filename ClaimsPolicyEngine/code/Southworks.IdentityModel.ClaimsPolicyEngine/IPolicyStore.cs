namespace Southworks.IdentityModel.ClaimsPolicyEngine
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Model;
    
    [ServiceContract]
    public interface IPolicyStore
    {
        [OperationContract]
        IEnumerable<PolicyScope> RetrieveScopes();

        [OperationContract]
        PolicyScope RetrieveScope(Uri scopeUri);

        [OperationContract]
        void AddPolicyRule(Uri scopeUri, PolicyRule rule);

        [OperationContract]
        void RemovePolicyRule(Uri scopeUri, PolicyRule rule);

        [OperationContract]
        void AddIssuer(Uri scopeUri, Issuer issuer);

        [OperationContract]
        void RemoveIssuer(Uri scopeUri, Issuer issuer);

        [OperationContract]
        Issuer RetrieveIssuer(Uri scopeUri, string issuerName);
    }
}