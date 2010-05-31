namespace Southworks.IdentityModel.ClaimsPolicyEngine.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PolicyScopeException : Exception
    {
        public PolicyScopeException()
        {
        }

        public PolicyScopeException(string message)
            : base(message)
        {
        }

        public PolicyScopeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PolicyScopeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}