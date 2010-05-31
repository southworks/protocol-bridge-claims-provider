<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="SampleRP.Library" %>
<%@ Import Namespace="System.IdentityModel" %>
<%@ Import Namespace="Microsoft.IdentityModel.Claims" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Secure
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Secure</h2>
    <h3>Welcome!</h3>
    <% foreach (Claim c in (ClaimCollection)ViewData["Claims"])
       { %>
    <strong><%= c.ClaimType %>:</strong> <%= c.Value%><br />
    <% } %>
</asp:Content>
