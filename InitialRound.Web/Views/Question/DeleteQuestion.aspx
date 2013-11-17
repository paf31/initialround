<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DeleteQuestion.aspx.cs" Inherits="InitialRound.Web.Views.Question.DeleteQuestion" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript" src="/Scripts/Question/DeleteQuestion.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new DeleteQuestion('<%= AntiForgeryToken %>', '<%= RouteData.Values["questionId"] %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">

    <h1>Delete Question</h1>

    <p>
        Are you sure you want to delete '<span data-bind="text: Name"></span>'? This operation cannot be undone.
    </p>

    <button type="button" class="btn btn-primary" data-bind="click: DeleteCommand">Delete Question</button>

</asp:Content>
