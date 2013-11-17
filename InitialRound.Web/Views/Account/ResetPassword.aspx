<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="InitialRound.Web.Views.Account.ResetPassword"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="/Scripts/Account/ResetPassword.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new ResetPassword());
        };
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="BodyContent">
    <h1>Reset Password</h1>

    <div data-bind="ifnot: Success">
        <p>In order to reset your password, please enter your username to identify yourself. An email will be sent to the email address associated with your account, which will contain instructions to reset your password.</p>

        <p>If you do not remember your username, please contact your administrator.</p>

        <div class="form-horizontal">
            <div class="control-group">
                <label class="control-label" for="inputUsr">Username</label>
                <div class="controls">
                    <input id="inputUsr" type="text" data-bind="value: Username" />
                </div>
            </div>
            <div class="control-group">
                <div class="controls">
                    <button type="submit" class="btn btn-primary" data-bind="click: ResetPasswordCommand">Send Email</button>
                </div>
            </div>
        </div>
    </div>

    <div data-bind="if: Success">
        <p>An email has been sent to the email address associated with your account. Please follow the instructions in the email to continue the password reset process.</p>

        <p>If you do not receive an email shortly, please contact your administrator.</p>
    </div>

</asp:Content>
