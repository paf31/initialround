<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompletePasswordReset.aspx.cs" Inherits="InitialRound.Web.Views.Account.CompletePasswordReset"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="/Scripts/Account/CompletePasswordReset.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new CompletePasswordReset('<%= Request["Token"] %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="BodyContent">
    <h1>Reset Password</h1>

    <div data-bind="ifnot: Success">
        <p>In order to complete the password reset process, please enter your username again, along with a new password.</p>

        <div class="form-horizontal">
            <div class="control-group">
                <label class="control-label" for="inputUsr">Username</label>
                <div class="controls">
                    <input id="inputUsr" type="text" data-bind="value: Username" />
                </div>
            </div>
            <div class="control-group">
                <label class="control-label" for="inputNp1">New Password</label>
                <div class="controls">
                    <input id="inputNp1" type="password" data-bind="value: NewPassword1" />
                </div>
            </div>
            <div class="control-group">
                <label class="control-label" for="inputNp2">Confirm New Password</label>
                <div class="controls">
                    <input id="inputNp2" type="password" data-bind="value: NewPassword2" />
                </div>
            </div>
            <div class="control-group">
                <div class="controls">
                    <button type="submit" class="btn btn-primary" data-bind="click: ResetPasswordCommand">Reset Password</button>
                </div>
            </div>
        </div>
    </div>

    <div data-bind="if: Success">
        <p>Your password has been successfully reset.</p>

        <p>You may now <a href="/">continue using your account</a>.</p>
    </div>

</asp:Content>
