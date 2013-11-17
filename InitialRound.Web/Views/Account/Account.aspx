<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="InitialRound.Web.Views.Account.Account"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="/Scripts/Account/Account.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new Account());
        };
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="BodyContent">
    <h1>Account</h1>
    <ul>
        <li><a href="/Account/ChangePassword">Change Password</a></li>
        <%  if (AuthToken.IsAdmin)
            {%>
        <li><a href="/Users">Manage Users</a></li>
        <li data-bind="visible: IsFreeAccount() === true"><a href="/Account/Convert">Convert To Paid Account</a></li>
        <li data-bind="visible: IsFreeAccount() === false"><a href="/Account/UpdatePaymentInfo">Update Payment Information</a></li>
        <li><a href="/Account/CloseAccount">Close Account</a></li>
        <% }%>
    </ul>
</asp:Content>
