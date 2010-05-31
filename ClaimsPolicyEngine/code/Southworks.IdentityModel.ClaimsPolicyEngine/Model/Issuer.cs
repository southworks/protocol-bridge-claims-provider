namespace Southworks.IdentityModel.ClaimsPolicyEngine.Model
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Issuer
    {
        public Issuer(string uri) : this(uri, string.Empty, uri) 
        {
        }

        public Issuer(string uri, string thumbprint)
            : this(uri, thumbprint, uri)
        {
        }

        public Issuer(string uri, string thumbprint, string displayName)
        {
            this.Uri = uri;
            this.DisplayName = displayName;
            this.Thumbprint = thumbprint;
        }

        [DataMember]
        public string Uri 
        { 
            get; set; 
        }

        [DataMember]
        public string DisplayName 
        { 
            get; set; 
        }

        [DataMember]
        public string Thumbprint 
        { 
            get; set; 
        }

        public override bool Equals(object obj)
        {
            var other = obj as Issuer;
            if (other == null)
            {
                return false;
            }

            return this.Uri.Equals(other.Uri);
        }

        public override int GetHashCode()
        {
            return this.Uri.GetHashCode();
        }
    }
}
