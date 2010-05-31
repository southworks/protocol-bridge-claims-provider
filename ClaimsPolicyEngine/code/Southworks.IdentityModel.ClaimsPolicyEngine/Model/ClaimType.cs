namespace Southworks.IdentityModel.ClaimsPolicyEngine.Model
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ClaimType
    {
        public ClaimType(string fullName)
        {
            this.FullName = fullName;
            this.DisplayName = fullName;
        }

        public ClaimType(string fullName, string displayName)
        {
            this.FullName = fullName;
            this.DisplayName = displayName;
        }

        [DataMember]
        public string FullName 
        { 
            get; set; 
        }

        public string DisplayName 
        { 
            get; set; 
        }

        public override bool Equals(object obj)
        {
            var other = obj as ClaimType;
            if (other == null)
            {
                return false;
            }

            return this.FullName.Equals(other.FullName);
        }

        public override int GetHashCode()
        {
            return this.FullName.GetHashCode();
        }
    }
}
