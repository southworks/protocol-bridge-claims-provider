namespace MultiProtocolIssuerSts.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Moq;

    public static class MvcMockHelpers
    {
        public static HttpContextBase FakeHttpContext()
        {

            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            
            var form = new NameValueCollection();
            var querystring = new NameValueCollection();
            var cookies = new HttpCookieCollection();

            request.Setup(r => r.Cookies).Returns(cookies);
            request.Setup(r => r.Form).Returns(form);
            request.Setup(q => q.QueryString).Returns(querystring);

            response.Setup(r => r.Cookies).Returns(cookies);


            return context.Object;
        }

        public static HttpContextBase FakeHttpContext(string url)
        {
            HttpContextBase context = FakeHttpContext();
            context.Request.SetupRequestUrl(url);

            return context;

        }


        public static void SetFakeControllerContext(this Controller controller)
        {
            var httpContext = FakeHttpContext();
            ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
        }

        public static void SetFakeControllerContext(this Controller controller, RouteData routeData)
        {
            SetFakeControllerContext(controller, new Dictionary<string, string>(), new HttpCookieCollection(), routeData);
        }

        public static void SetFakeControllerContext(this Controller controller, HttpCookieCollection requestCookies)
        {
            SetFakeControllerContext(controller, new Dictionary<string, string>(), requestCookies, new RouteData());
        }

        public static void SetFakeControllerContext(this Controller controller, Dictionary<string, string> formValues)
        {
            SetFakeControllerContext(controller, formValues, new HttpCookieCollection(), new RouteData());
        }

        public static void SetFakeControllerContext(this Controller controller,
            Dictionary<string, string> formValues,
            HttpCookieCollection requestCookies,
            RouteData routeData)
        {
            var httpContext = FakeHttpContext();

            foreach (string key in formValues.Keys)
            {
                httpContext.Request.Form.Add(key, formValues[key]);

            }
            foreach (string key in requestCookies.Keys)
            {
                httpContext.Request.Cookies.Add(requestCookies[key]);

            }
            ControllerContext context = new ControllerContext(new RequestContext(httpContext, routeData), controller);
            controller.ControllerContext = context;
        }
        
        static NameValueCollection GetQueryStringParameters(string url)
        {
            if (url.Contains("?"))
            {
                NameValueCollection parameters = new NameValueCollection();

                string[] parts = url.Split("?".ToCharArray());
                string[] keys = parts[1].Split("&".ToCharArray());

                foreach (string key in keys)
                {
                    string[] part = key.Split("=".ToCharArray());
                    parameters.Add(part[0], part[1]);
                }

                return parameters;
            }
            else
            {
                return null;
            }
        }

        public static void SetHttpMethodResult(this HttpRequestBase request, string httpMethod)
        {
            Mock.Get(request)
                .Setup(req => req.HttpMethod)
                .Returns(httpMethod);
        }

        public static void SetAnonymousUser(this HttpContextBase context)
        {
            var mock = Mock.Get(context);            
            var principal = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            principal.Setup(p => p.Identity).Returns(identity.Object);
            identity.Setup(i => i.IsAuthenticated).Returns(false);
            mock.Setup(u => u.User).Returns(principal.Object);                       
        }

        public static void SetupRequestUrl(this HttpRequestBase request, string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            
            var headers = new NameValueCollection();
            headers.Add("HOST", new Uri(url).Host);

            var mock = Mock.Get(request);

            mock.Setup(req => req.Url)
                .Returns(new Uri(url));
            mock.Setup(req => req.QueryString)
                .Returns(GetQueryStringParameters(url));
            mock.Setup(req => req.PathInfo)
                .Returns(string.Empty);
            mock.Setup(req => req.Headers)
                .Returns(headers);
        }        
    }
}
