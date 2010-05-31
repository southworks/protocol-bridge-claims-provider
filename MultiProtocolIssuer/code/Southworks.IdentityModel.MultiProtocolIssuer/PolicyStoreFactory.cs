namespace Southworks.IdentityModel.MultiProtocolIssuer
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;
    using Southworks.IdentityModel.ClaimsPolicyEngine;

    public class PolicyStoreFactory : ServiceHostFactory
    {
        private static readonly object LockObject = new object();
        private static IPolicyStore policyStoreInstance;

        public static IPolicyStore Instance
        {
            get
            {
                if (policyStoreInstance == null)
                {
                    lock (LockObject)
                    {
                        if (policyStoreInstance == null)
                        {
                            policyStoreInstance = new XmlPolicyStore(HttpContext.Current.Server.MapPath(@"~\App_Data\claimsPolicies.xml"), new FileXmlRepository());
                        }
                    }
                }

                return policyStoreInstance;
            }
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new ServiceHost(Instance, baseAddresses);
        }
    }
}
