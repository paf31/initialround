<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListUsers.aspx.cs" Inherits="InitialRound.Web.Views.Account.ListUsers" %>

<%@ Import Namespace="InitialRound.BusinessLogic" %>
<%@ Register Src="~/Controls/List.ascx" TagName="List" TagPrefix="controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/ListViewModel.js"></script>
    <script type="text/javascript" src="/Scripts/Users/ListUsers.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new ListUsers);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h2>Users</h2>

    <div>
        <a class="btn" href="/Users/Create"><span class="icon-plus"></span> New User</a>
    </div>

    <controls:List runat="server" />
</asp:Content>
