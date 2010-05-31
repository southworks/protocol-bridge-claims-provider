namespace Southworks.IdentityModel.ClaimsPolicyEngine.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ClaimsPolicyEvaluationException : Exception
    {
        public ClaimsPolicyEvaluationException()
        {
        }

        public ClaimsPolicyEvaluationException(string message)
            : base(message)
        {
        }

        public ClaimsPolicyEvaluationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ClaimsPolicyEvaluationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}