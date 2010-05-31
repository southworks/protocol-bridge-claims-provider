namespace Southworks.IdentityModel.ClaimsPolicyEngine.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PolicyClaimException : Exception
    {
        public PolicyClaimException()
        {
        }

        public PolicyClaimException(string message)
            : base(message)
        {
        }

        public PolicyClaimException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PolicyClaimException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}