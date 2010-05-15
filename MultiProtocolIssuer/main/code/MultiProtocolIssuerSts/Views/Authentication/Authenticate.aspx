<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Multi Protocol Security Token Service
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Multi Protocol Security Token Service</h2>
    <form action="" method="get" id="openid_form">
    <input type="hidden" name="action" value="verify" />
    <fieldset>
        <legend>Sign-in</legend>
        <div id="openid_choice" style="display: block;">
            <p>
                Please click your account provider:</p>
            <div id="openid_btns">                
                    <a class="yahoo openid_large_btn"
                        style="background: url(&quot;Content/images/yahoo.gif&quot;) no-repeat scroll center center rgb(255, 255, 255);"
                        href="authenticate?whr=urn:Yahoo:AX" title="Yahoo"></a>                    
                    <a class="openid_large_btn"
                        style="background: url(&quot;Content/images/liveid.png&quot;) no-repeat scroll center center rgb(255, 255, 255);"
                        href="authenticate?whr=urn:LiveId" title="Winodws Live"></a>                    
                        <a class="openid_large_btn"
                        style="background: url(&quot;Content/images/facebook.gif&quot;) no-repeat scroll center center rgb(255, 255, 255);"
                        href="authenticate?whr=urn:Facebook" title="Facebook"></a>                                    
            </div>
        </div>
        <noscript>
            <p>
                OpenID is service that allows you to log-on to many different websites using a single
                indentity. Find out <a href="http://openid.net/what/">more about OpenID</a> and
                <a href="http://openid.net/get/">how to get an OpenID enabled account</a>.</p>
        </noscript>
    </fieldset>    
    <input type="hidden" value="<%=HttpContext.Current.Request.QueryString["ReturnUrl"] %>" />
    </form>
</asp:Content>
<asp:Content ID="pageSpecificScripts" ContentPlaceHolderID="PageSpecificScripts"
    runat="server">    

</asp:Content>
