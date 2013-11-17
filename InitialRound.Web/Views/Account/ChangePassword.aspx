<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs"
    Inherits="InitialRound.Web.Views.Account.ChangePassword" MasterPageFile="~/Site.master" %>

<%@ Import Namespace="InitialRound.BusinessLogic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript" src="/Scripts/Account/ChangePassword.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new ChangePassword("<%= AntiForgeryToken %>"));
        };
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="BodyContent">
    <h1>Change Password</h1>

    <div data-bind="ifnot: Success">
        <div class="form-horizontal">
            <div class="control-group">
                <label class="control-label" for="inputOp">Old Password</label>
                <div class="controls">
                    <input id="inputOp" type="password" data-bind="value: OldPassword" />
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
                    <button type="submit" class="btn" data-bind="click: ChangePasswordCommand">Change Password</button>
                </div>
            </div>
        </div>
    </div>

    <div data-bind="if: Success">
        <p>Your password has been successfully changed.</p>
    </div>
</asp:Content>
