<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DeleteApplicant.aspx.cs" Inherits="InitialRound.Web.Views.Applicant.DeleteApplicant" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript" src="/Scripts/Applicant/DeleteApplicant.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new DeleteApplicant('<%= AntiForgeryToken %>', '<%= RouteData.Values["applicantId"] %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h1>Delete Applicant</h1>
    <p>
        Are you sure you want to delete '<span data-bind="text: FirstName"></span> <span data-bind="text: LastName"></span>'? This operation cannot be undone.
    </p>
    <p>
        All associated interviews will also be deleted.
    </p>

    <button type="button" class="btn btn-primary" data-bind="click: DeleteCommand">Delete Applicant</button>

</asp:Content>
