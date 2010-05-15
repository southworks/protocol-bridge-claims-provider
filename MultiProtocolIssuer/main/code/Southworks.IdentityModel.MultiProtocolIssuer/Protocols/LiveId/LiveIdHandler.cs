namespace Southworks.IdentityModel.MultiProtocolIssuer.Protocols.LiveId
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using Microsoft.IdentityModel.Claims;

    using Southworks.IdentityModel.MultiProtocolIssuer.Model;

    using WindowsLive;

    public class LiveIdHandler : ProtocolHandlerBase
    {
        private readonly ClaimProvider issuer;
        private readonly Uri liveIdBaseUrl;
        private readonly string appId;
        private readonly string algorithm;
        private readonly string secretKey;

        public LiveIdHandler(ClaimProvider issuer) : base(issuer)
        {
            if (issuer == null)
                throw new ArgumentNullException("issuer");

            this.issuer = issuer;
            this.liveIdBaseUrl = this.issuer.Url;
            this.appId = this.issuer.Parameters["wll_appid"];
            this.algorithm = this.issuer.Parameters["wll_securityalgorithm"];
            this.secretKey = this.issuer.Parameters["wll_secret"];
        }

        public override void ProcessSignInRequest(Scope scope, HttpContextBase httpContext)
        {
            var liveIdUrl = string.Format("{0}?appid={1}&alg={2}&appctx={3}", this.liveIdBaseUrl, this.appId, this.algorithm, string.Empty);

            httpContext.Response.Redirect(liveIdUrl, false);
            httpContext.ApplicationInstance.CompleteRequest();
        }

        public override IClaimsIdentity ProcessSignInResponse(string realm, string originalUrl, HttpContextBase httpContext)
        {
            var windowLiveLogin = new WindowsLiveLogin(this.appId, this.secretKey, this.algorithm);
            var user = windowLiveLogin.ProcessLogin(httpContext.Request.Form);
            
            var claims = new List<Claim>
                {
                    new Claim(System.IdentityModel.Claims.ClaimTypes.Name, user.Id)
                };

            return new ClaimsIdentity(claims, "LiveId");
        }      
    }
}