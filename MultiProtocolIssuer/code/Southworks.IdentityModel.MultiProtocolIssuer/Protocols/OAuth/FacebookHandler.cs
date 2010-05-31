namespace Southworks.IdentityModel.MultiProtocolIssuer.Protocols.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;

    using Microsoft.Http;
    using Microsoft.IdentityModel.Claims;

    using Newtonsoft.Json.Linq;

    using Southworks.IdentityModel.MultiProtocolIssuer.Model;

    public class FacebookHandler : ProtocolHandlerBase
    {
        private readonly ClaimProvider issuer;

        private readonly string applicationId;

        private readonly string apiUrl;

        private readonly string secret;

        private readonly string apiKey;

        public FacebookHandler(ClaimProvider issuer)
            : base(issuer)
        {
            this.issuer = issuer;
            this.applicationId = issuer.Parameters["application_id"];
            this.apiUrl = issuer.Parameters["api_url"];
            this.apiKey = issuer.Parameters["api_key"];
            this.secret = issuer.Parameters["secret"];
        }

        public override void ProcessSignInRequest(Scope scope, HttpContextBase httpContext)
        {
            var extendedPermissions = GetExtendedPermissions(scope);

            var loginUrl = string.Format(
                                 CultureInfo.InvariantCulture,
                                 "{0}?client_id={1}&redirect_uri={2}&scope={3}",
                                 this.issuer.Url,
                                 this.applicationId,
                                 HttpUtility.UrlEncode(this.MultiProtocolIssuer.ReplyUrl.ToString()),
                                 String.Join(" ", extendedPermissions));

            httpContext.Response.Redirect(loginUrl);
            httpContext.ApplicationInstance.CompleteRequest();
        }

        public override IClaimsIdentity ProcessSignInResponse(string realm, string originalUrl, HttpContextBase httpContext)
        {
            var verificationCode = httpContext.Request["code"];
            var accessToken = this.GetAccessToken(verificationCode);

            return this.GetUserClaims(accessToken);
        }

        private static string[] GetExtendedPermissions(Scope scope)
        {
            // Complete permission list
            // http://developers.facebook.com/docs/authentication/permissions
            var permissions = new List<string>();

            foreach (var requirement in scope.ClaimTypeRequirements)
            {
                switch (requirement.ClaimType)
                {
                    case "http://schema.facebook.com/me/email":
                        permissions.Add("email");
                        break;
                }
            }

            return permissions.ToArray();
        }

        private string GetAccessToken(string verificationCode)
        {
            var getAccessTokenUrl = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/oauth/access_token", this.apiUrl));
            var queryString = new HttpQueryString
                {
                    { "client_id", this.applicationId },
                    { "redirect_uri", this.MultiProtocolIssuer.ReplyUrl.ToString() },
                    { "client_secret", this.secret },
                    { "code", verificationCode }
                };

            using (var client = new HttpClient())
            {
                using (var response = client.Get(getAccessTokenUrl, queryString).EnsureStatusIsSuccessful())
                {
                    var content = response.Content.ReadAsString();
                    var qs = HttpUtility.ParseQueryString(content);

                    return qs["access_token"];
                }
            }
        }

        private IClaimsIdentity GetUserClaims(string accessToken)
        {
            var userProfileUrl = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/me", this.apiUrl));

            var queryString = new HttpQueryString { { "access_token", accessToken } };

            List<Claim> claims;

            using (var client = new HttpClient())
            {
                using (var response = client.Get(userProfileUrl, queryString).EnsureStatusIsSuccessful())
                {
                    var userProfile = response.Content.ReadAsString();
                    var profile = JObject.Parse(userProfile);

                    claims = this.ExtractClaimsFromJson("http://schema.facebook.com/me/", profile.Root.Children());
                }
            }

            return new ClaimsIdentity(claims, "OAuth");
        }

        private List<Claim> ExtractClaimsFromJson(string parentClaimType, IEnumerable<JToken> items)
        {
            var claims = new List<Claim>();

            foreach (var item in items)
            {
                if (!item.First.HasValues)
                {
                    claims.Add(new Claim(parentClaimType + ((JProperty)item).Name, ((JProperty)item).Value.ToString()));
                }
                else
                {
                    var claimType = parentClaimType;
                    if (item.GetType().Name == "JProperty")
                    {
                        claimType = claimType + ((JProperty)item).Name + "_";
                    }

                    claims.AddRange(this.ExtractClaimsFromJson(claimType, item));
                }
            }

            return claims;
        }        
    }
}
