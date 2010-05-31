namespace Southworks.IdentityModel.MultiProtocolIssuer.Model
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    public class MultiProtocolIssuer
    {
        public Uri Identifier { get; set; }
        
        public Uri ReplyUrl { get; set; }

        public X509Certificate2 SigningCertificate { get; set; }
    }
}
