<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListApplicants.aspx.cs" Inherits="InitialRound.Web.Views.Applicant.ListApplicants" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<%@ Register Src="~/Controls/List.ascx" TagName="List" TagPrefix="controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/ListViewModel.js"></script>
    <script type="text/javascript" src="/Scripts/Applicant/ListApplicants.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new ListApplicants);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h2>Applicants</h2>

    <p>
        <a class="btn" href="/Applicants/Create"><span class="icon-plus"></span> New Applicant</a>
        <a class="btn" href="/Applicants/BulkImport"><span class="icon-file"></span> Bulk Import</a>
    </p>

    <controls:List runat="server" />
</asp:Content>
