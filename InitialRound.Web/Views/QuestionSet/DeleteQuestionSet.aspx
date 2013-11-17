<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DeleteQuestionSet.aspx.cs" Inherits="InitialRound.Web.Views.QuestionSet.DeleteQuestionSet" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript" src="/Scripts/QuestionSet/DeleteQuestionSet.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new DeleteQuestionSet('<%= AntiForgeryToken %>', '<%= RouteData.Values["questionSetId"] %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h2>Delete Question Set</h2>

    <p>
        Are you sure you want to delete '<span data-bind="text: Name"></span>'? This operation cannot be undone.
    </p>

    <button type="button" class="btn btn-primary" data-bind="click: DeleteCommand">Delete Question Set</button>
</asp:Content>
