<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DeleteUser.aspx.cs" Inherits="InitialRound.Web.Views.Users.DeleteUser" %>

<%@ Import Namespace="InitialRound.BusinessLogic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Users/DeleteUser.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new DeleteUser('<%= AntiForgeryToken %>', '<%= RouteData.Values["username"] %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h1 class="legend">Delete User</h1>

    <p>Are you sure you want to delete this user? This operation cannot be undone.</p>

    <button type="button" class="btn btn-primary" data-bind="click: DeleteCommand">Delete User</button>
</asp:Content>
