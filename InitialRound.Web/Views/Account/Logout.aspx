<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="InitialRound.Web.Views.Account.Logout"
    MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new ViewModel());
        };
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="BodyContent">
    <h1>Log Out</h1>
    <p>
        You have been logged out.</p>
    <p>
        <a href="/Login">Login</a></p>
</asp:Content>
