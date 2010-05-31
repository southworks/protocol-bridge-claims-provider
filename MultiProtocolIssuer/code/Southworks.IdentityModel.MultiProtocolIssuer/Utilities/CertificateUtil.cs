namespace Southworks.IdentityModel.MultiProtocolIssuer.Utilities
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    public class CertificateUtil
    {
        public static X509Certificate2 GetCertificate(StoreName name, StoreLocation location, string subjectName)
        {
            X509Store store = new X509Store(name, location);
            X509Certificate2Collection certificates = null;
            store.Open(OpenFlags.ReadOnly);

            try
            {
                X509Certificate2 result = null;

                certificates = store.Certificates;

                for (int i = 0; i < certificates.Count; i++)
                {
                    X509Certificate2 cert = certificates[i];

                    if (cert.SubjectName.Name.ToLower() == subjectName.ToLower())
                    {
                        if (result != null)
                        {
                            throw new ApplicationException(string.Format("There are multiple certificates for subject Name {0}", subjectName));
                        }

                        result = new X509Certificate2(cert);
                    }
                }

                if (result == null)
                {
                    throw new ApplicationException(string.Format("No certificate was found for subject Name {0}", subjectName));
                }

                return result;
            }
            finally
            {
                if (certificates != null)
                {
                    for (int i = 0; i < certificates.Count; i++)
                    {
                        X509Certificate2 cert = certificates[i];
                        cert.Reset();
                    }
                }

                store.Close();
            }
        }
    }
}