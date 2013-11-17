<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="InitialRound.Web.Views.Account.Login"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="/Scripts/Account/Login.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new Login());
        };
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="BodyContent">
    <h1>Login</h1>

    <div class="form-horizontal">
        <div class="control-group">
            <label class="control-label" for="inputUsr">Username</label>
            <div class="controls">
                <input id="inputUsr" type="text" data-bind="value: Username" />
            </div>
        </div>
        <div class="control-group">
            <label class="control-label" for="inputPwd">Password</label>
            <div class="controls">
                <input id="inputPwd" type="password" data-bind="value: Password" />
            </div>
        </div>
        <div class="control-group">
            <div class="controls">
                <button type="submit" class="btn btn-primary" data-bind="click: LoginCommand">Login</button>
                <div><a href="/Account/ResetPassword"><small>Forgotten your password?</small></a></div>
            </div>
        </div>
    </div>

</asp:Content>
