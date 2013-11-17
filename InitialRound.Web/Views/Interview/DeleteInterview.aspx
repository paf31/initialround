<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DeleteInterview.aspx.cs" Inherits="InitialRound.Web.Views.Interview.DeleteInterview" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript" src="/Scripts/Interview/DeleteInterview.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new DeleteInterview('<%= AntiForgeryToken %>', '<%= RouteData.Values["interviewId"] %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h1>Delete Interview</h1>

    <p>
        Are you sure you want to delete this interview? This operation cannot be undone.
    </p>

    <button type="button" class="btn btn-primary" data-bind="click: DeleteCommand">Delete Interview</button>

</asp:Content>
