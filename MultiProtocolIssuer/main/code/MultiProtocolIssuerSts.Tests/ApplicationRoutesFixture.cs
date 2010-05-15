namespace MultiProtocolIssuerSts.Tests
{
    using System.Web;
    using System.Web.Routing;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class ApplicationRoutesFixture
    {
        [TestMethod]
        public void ShouldRouteToTheAuthenticationAction()
        {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath)
                           .Returns("~/");

            var routeData = routes.GetRouteData(httpContextMock.Object);
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Authentication", routeData.Values["Controller"]);
            Assert.AreEqual("ProcessFederationRequest", routeData.Values["Action"]);
        }

        [TestMethod]
        public void ShouldRouteToTheAuthenticateAction()
        {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath)
                           .Returns("~/authenticate");

            var routeData = routes.GetRouteData(httpContextMock.Object);
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Authentication", routeData.Values["Controller"]);
            Assert.AreEqual("Authenticate", routeData.Values["Action"]);
        }

        [TestMethod]
        public void ShouldRouteToTheProcessResponseAction()
        {
            var routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath)
                           .Returns("~/response");

            var routeData = routes.GetRouteData(httpContextMock.Object);
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Authentication", routeData.Values["Controller"]);
            Assert.AreEqual("ProcessResponse", routeData.Values["Action"]);
        }
    }
}
