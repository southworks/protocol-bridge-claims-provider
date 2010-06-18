namespace Southworks.IdentityModel.MultiProtocolIssuer.Protocols.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Xml.Linq;
    using DotNetOpenAuth.Messaging;
    using DotNetOpenAuth.OAuth;
    using DotNetOpenAuth.OAuth.ChannelElements;
    using Microsoft.Http;
    using Microsoft.IdentityModel.Claims;
    using Southworks.IdentityModel.MultiProtocolIssuer.Model;

    public class TwitterHandler : ProtocolHandlerBase
    {
        private readonly ClaimProvider issuer;
        private readonly string consumerKey;
        private readonly string consumerSecret;
        private readonly string apiUrl;
        private readonly WebConsumer consumer;
        private static readonly IDictionary<string, string> ClaimTypesMapping;
        private const string RestUrl = "http://twitter.com";
        private const string RequestTokenUrl = "http://twitter.com/oauth/request_token";
        private const string AuthorizeUrl = "http://twitter.com/oauth/authorize";
        private const string AccessTokenUrl = "http://twitter.com/oauth/access_token";

        static TwitterHandler()
        {
            ClaimTypesMapping = new Dictionary<string, string>();

            ClaimTypesMapping["id"] = "http://twitter.com/claims/id";
            ClaimTypesMapping["name"] = "http://twitter.com/claims/name";
            ClaimTypesMapping["screen_name"] = "http://twitter.com/claims/screen_name";
            ClaimTypesMapping["location"] = "http://twitter.com/claims/location";
            ClaimTypesMapping["description"] = "http://twitter.com/claims/description";
            ClaimTypesMapping["profile_image_url"] = "http://twitter.com/claims/profile_image_url";
            ClaimTypesMapping["url"] = "http://twitter.com/claims/url";
            ClaimTypesMapping["created_on"] = "http://twitter.com/claims/created_on";
            ClaimTypesMapping["utc_offset"] = "http://twitter.com/claims/utc_offset";
            ClaimTypesMapping["lang"] = "http://twitter.com/claims/lang";
        }

        public TwitterHandler(ClaimProvider issuer)
            : base(issuer)
        {
            this.issuer = issuer;
            this.consumerKey = issuer.Parameters["consumer_key"];
            this.consumerSecret = issuer.Parameters["consumer_secret"];
            this.apiUrl = issuer.Parameters["api_url"];

            var description = new ServiceProviderDescription
            {
                RequestTokenEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/request_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
                UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/authenticate", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
                AccessTokenEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/access_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
                TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() }
            };
        }

        public override void ProcessSignInRequest(Scope scope, HttpContextBase httpContext)
        {
            httpContext.ApplicationInstance.CompleteRequest();

            HttpContext.Current.Response.Redirect(this.GetAuthorizationLink());
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public override IClaimsIdentity ProcessSignInResponse(string realm, string originalUrl, HttpContextBase httpContext)
        {
            var accessToken = this.GetAccessToken(httpContext.Request["oauth_token"]);
            var user = XDocument.Parse(this.VerifyCredentials(accessToken));

            var dataDictionary = GetDictionary(
                user.Element("user").Elements(),
                ClaimTypesMapping.Keys);

            var identity = new ClaimsIdentity();

            foreach (var entry in dataDictionary)
            {
                identity.Claims.Add(new Claim(ClaimTypesMapping[entry.Key], entry.Value));
            }

            return identity;
        }

        private static string EncodePostData(string postData)
        {
            NameValueCollection qs = HttpUtility.ParseQueryString(postData);
            string encoded = string.Empty;

            foreach (string key in qs.AllKeys)
            {
                if (encoded.Length > 0)
                {
                    encoded += "&";
                }

                qs[key] = HttpUtility.UrlDecode(qs[key]);
                qs[key] = OAuthHelper.UrlEncode(qs[key]);
                encoded += key + "=" + qs[key];
            }

            return encoded;
        }

        private static IDictionary<string, string> GetDictionary(IEnumerable<XElement> nodes, IEnumerable<string> nodeNames)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var node in nodeNames)
            {
                var element = nodes.SingleOrDefault(n => n.Name.ToString().Equals(node, StringComparison.OrdinalIgnoreCase));

                if (element != null)
                {
                    dictionary[node] = element.Value;
                }
            }

            return dictionary;
        }

        private string GetAuthorizationLink()
        {
            string link = string.Empty;

            using (var response = this.OAuthWebRequest(string.Empty, string.Empty, "GET", RequestTokenUrl, String.Empty))
            {
                var content = response.Content.ReadAsString();

                if (content.Length > 0)
                {
                    NameValueCollection qs = HttpUtility.ParseQueryString(content);
                    if (qs["oauth_token"] != null)
                    {
                        link = AuthorizeUrl + "?oauth_token=" + qs["oauth_token"];
                    }
                }

                return link;
            }
        }

        private AccessToken GetAccessToken(string requestToken)
        {
            using (var response = this.OAuthWebRequest(requestToken, string.Empty, "GET", AccessTokenUrl, String.Empty))
            {
                var content = response.Content.ReadAsString();

                if (content.Length > 0)
                {
                    NameValueCollection qs = HttpUtility.ParseQueryString(content);

                    return new AccessToken
                    {
                        Token = qs["oauth_token"],
                        TokenSecret = qs["oauth_token_secret"]
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        private string VerifyCredentials(AccessToken accessToken)
        {
            var url = string.Format(CultureInfo.InvariantCulture, @"{0}/account/verify_credentials.xml", RestUrl);

            using (var response = this.OAuthWebRequest(accessToken.Token, accessToken.TokenSecret, "GET", url, string.Empty))
            {
                return response.Content.ReadAsString();
            }
        }

        private HttpResponseMessage OAuthWebRequest(string token, string tokenSecret, string method, string url, string postData)
        {
            string outUrl = string.Empty;
            string querystring = string.Empty;

            if (string.Equals(method, "POST"))
            {
                if (postData.Length > 0)
                {
                    if (url.IndexOf("?") > 0)
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }

                    url += EncodePostData(postData);
                }
            }

            Uri uri = new Uri(url);

            string nonce = OAuthHelper.GenerateNonce();
            string timeStamp = OAuthHelper.GenerateTimeStamp();

            string signature = OAuthHelper.GenerateSignature(
                uri,
                this.consumerKey,
                this.consumerSecret,
                token,
                tokenSecret,
                method.ToString(),
                timeStamp,
                nonce,
                out outUrl,
                out querystring);

            querystring += "&oauth_signature=" + HttpUtility.UrlEncode(signature);

            if (string.Equals(method, "POST"))
            {
                postData = querystring;
                querystring = string.Empty;
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            return this.WebRequest(method, outUrl + querystring, postData);
        }

        private HttpResponseMessage WebRequest(string method, string url, string postData)
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            using (HttpClient client = new HttpClient())
            {
                if (string.Equals(method, "POST"))
                {
                    using (var body = HttpContent.Create(postData, "application/x-www-form-urlencoded"))
                    {
                        return client.Post(url, body).EnsureStatusIsSuccessful();
                    }
                }
                else
                {
                    return client.Get(url).EnsureStatusIsSuccessful();
                }
            }
        }
    }
}
