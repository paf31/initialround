<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListInterviews.aspx.cs" Inherits="InitialRound.Web.Views.Interview.ListInterviews" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<%@ Register Src="~/Controls/List.ascx" TagName="List" TagPrefix="controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/ListViewModel.js"></script>
    <script type="text/javascript" src="/Scripts/Interview/ListInterviews.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new ListInterviews(<%= Request["Status"] ?? "null" %>));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h2>Interviews</h2>

    <div class="alert alert-info">Create a new interview from the applicant's details page, or by using the Quick Create tool on the <a href="/">Dashboard</a>.</div>

    <controls:List runat="server" />
</asp:Content>
