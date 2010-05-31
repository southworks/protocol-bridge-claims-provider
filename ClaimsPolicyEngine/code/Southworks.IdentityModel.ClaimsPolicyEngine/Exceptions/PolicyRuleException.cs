namespace Southworks.IdentityModel.ClaimsPolicyEngine.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PolicyRuleException : Exception
    {
        public PolicyRuleException()
        {
        }

        public PolicyRuleException(string message)
            : base(message)
        {
        }

        public PolicyRuleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PolicyRuleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}