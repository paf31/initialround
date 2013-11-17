<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListQuestions.aspx.cs" Inherits="InitialRound.Web.Views.Question.ListQuestions" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<%@ Register Src="~/Controls/List.ascx" TagName="List" TagPrefix="controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/ListViewModel.js"></script>
    <script type="text/javascript" src="/Scripts/Question/ListQuestions.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new ListQuestions);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h1>Questions</h1>

    <div>
        <a class="btn" href="/Questions/Create"><span class="icon-plus"></span> New Question</a>
    </div>

    <controls:List ID="List1" runat="server" />
</asp:Content>
