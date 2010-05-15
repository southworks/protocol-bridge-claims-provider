namespace Southworks.IdentityModel.MultiProtocolIssuer.Tests.ProtocolHandlers
{
    using System;
    using System.Configuration;
    using System.Web;

    using DotNetOpenAuth.OpenId;
    using DotNetOpenAuth.OpenId.Messages;
    using DotNetOpenAuth.OpenId.RelyingParty;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Southworks.IdentityModel.MultiProtocolIssuer.Model;
    using Southworks.IdentityModel.MultiProtocolIssuer.Protocols.OpenID;

    [TestClass]
    public class OpenIdAxHandlerFixture
    {        
        [TestMethod]
        public void ShouldCallToCreateRequestOnTheOpenIdRelyingPartyWhenProcessSignInRequest()
        {
            var mockRelyingParty = new Mock<IOpenIdRelyingPartyFacade>();
            var mockAuthenticationRequest = new Mock<IAuthenticationRequest>();

            mockAuthenticationRequest.Setup(r => r.AddExtension(It.IsAny<IOpenIdMessageExtension>())).Verifiable();
            mockAuthenticationRequest.Setup(r => r.RedirectToProvider()).Verifiable();

            var providerUrl = new Uri("https://providerurl0");
            var scope = new Uri("https://relyingpartyidentifier");

            mockRelyingParty.Setup(
                x =>
                x.CreateRequest(
                    It.Is<Identifier>(s => s.ToString() == providerUrl.ToString()),
                    It.Is<Realm>(s => s.ToString() == "https://stsrealm/"),
                    It.Is<Uri>(s => s.ToString() == "https://response-endpoint/")))
            .Returns(() => mockAuthenticationRequest.Object);

            var handler = new OpenIdAxHandler(new ClaimProvider { Identifier = new Uri("urn:provider0"), Url = providerUrl }, mockRelyingParty.Object);
            handler.ProcessSignInRequest(
                new Scope
                {
                    Identifier = scope,
                    ClaimTypeRequirements = new ClaimTypeRequirement[] 
                    { 
                        new ClaimTypeRequirement { ClaimType = "type", DemandLevel = ClaimDemandLevel.Request } 
                    }
                }, new Mock<HttpContextBase>().Object);
        }
    }
}
