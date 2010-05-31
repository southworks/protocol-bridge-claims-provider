namespace Southworks.IdentityModel.ClaimsPolicyEngine.Model
{
    using System.Runtime.Serialization;

    [DataContract]
    public class InputPolicyClaim : PolicyClaim
    {
        public InputPolicyClaim(Issuer issuer, ClaimType claimType, string value)
            : base(claimType, value)
        {
            this.Issuer = issuer;
        }

        [DataMember]
        public Issuer Issuer
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            var other = obj as InputPolicyClaim;
            if (other == null)
            {
                return false;
            }

            return base.Equals(obj) && this.Issuer.Equals(other.Issuer);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                return base.GetHashCode() + this.Issuer.GetHashCode();   
            }
        }
    }
}