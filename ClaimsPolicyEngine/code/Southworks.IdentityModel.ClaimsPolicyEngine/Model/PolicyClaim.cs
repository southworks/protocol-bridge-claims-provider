namespace Southworks.IdentityModel.ClaimsPolicyEngine.Model
{
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class PolicyClaim
    {
        protected PolicyClaim(ClaimType claimType, string value)
        {
            this.ClaimType = claimType;
            this.Value = value;
        }

        [DataMember]
        public ClaimType ClaimType
        {
            get;
            private set;
        }

        [DataMember]
        public string Value
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PolicyClaim;
            if (other == null)
            {
                return false;
            }

            return this.ClaimType.Equals(other.ClaimType) && this.Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.ClaimType.GetHashCode() + this.Value.GetHashCode();
            }
        }
    }
}