namespace Southworks.IdentityModel.ClaimsPolicyEngine.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Exceptions;
    using Southworks.IdentityModel.ClaimsPolicyEngine.Properties;

    [DataContract]
    public enum AssertionsMatch
    {
        [EnumMember]
        NotSet = 0,

        [EnumMember]
        Any = 1,

        [EnumMember]
        All = 2
    }

    [DataContract]
    public class PolicyRule
    {
        public PolicyRule(AssertionsMatch assertionsMatch, IEnumerable<InputPolicyClaim> inputClaims, OutputPolicyClaim outputClaim)
        {
            if (outputClaim.CopyFromInput && inputClaims.Count() > 1)
            {
                throw new PolicyRuleException(Resources.CopyFromInputWithMultipleInputClaims);
            }

            this.AssertionsMatch = assertionsMatch;            
            this.OutputClaim = outputClaim;
            this.InputClaims = new List<InputPolicyClaim>();
            this.InputClaims.AddRange(inputClaims);
        }

        [DataMember]
        public AssertionsMatch AssertionsMatch
        { 
            get; private set; 
        }

        [DataMember]
        public List<InputPolicyClaim> InputClaims
        {
            get;
            set;
        }

        [DataMember]
        public OutputPolicyClaim OutputClaim 
        { 
            get; private set; 
        }

        public override bool Equals(object obj)
        {
            var other = obj as PolicyRule;
            if (other == null)
            {
                return false;
            }

            if (!this.OutputClaim.Equals(other.OutputClaim) || 
                !this.AssertionsMatch.Equals(other.AssertionsMatch) ||
                (this.InputClaims.Count != other.InputClaims.Count))
            {
                return false;
            }

            foreach (var inputClaim in this.InputClaims)
            {
                if (!other.InputClaims.Contains(inputClaim))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return this.OutputClaim.GetHashCode();   
        }
    }
}