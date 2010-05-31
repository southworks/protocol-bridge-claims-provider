namespace Southworks.IdentityModel.ClaimsPolicyEngine.Model
{
    using System.Runtime.Serialization;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Exceptions;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Properties;

    [DataContract]
    public class OutputPolicyClaim : PolicyClaim
    {
        public OutputPolicyClaim(ClaimType claimType, string value)
            : this(claimType, value, string.Empty)
        {
            this.CopyFrom = string.Empty;
        }

        public OutputPolicyClaim(ClaimType claimType, string value, string copyFrom)
            : base(claimType, value)
        {
            if (value == "*")
            {
                throw new PolicyClaimException(Resources.WildcardOnOutputClaim);
            }

            if (!string.IsNullOrEmpty(copyFrom) && !string.IsNullOrEmpty(value))
            {
                throw new PolicyClaimException(Resources.CopyFromInputAndValueSet);
            }

            if (string.IsNullOrEmpty(copyFrom) && string.IsNullOrEmpty(value))
            {
                throw new PolicyClaimException(Resources.NoOutputValueSet);
            }

            this.CopyFrom = copyFrom;
        }

        [DataMember]
        public string CopyFrom 
        { 
            get; private set; 
        }

        public bool CopyFromInput
        {
            get
            {
                return !string.IsNullOrEmpty(this.CopyFrom);
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as OutputPolicyClaim;
            if (other == null)
            {
                return false;
            }

            return base.Equals(obj) && this.CopyFrom.ToUpperInvariant().Equals(other.CopyFrom.ToUpperInvariant());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return base.GetHashCode() + this.CopyFrom.GetHashCode();
            }
        }
    }
}