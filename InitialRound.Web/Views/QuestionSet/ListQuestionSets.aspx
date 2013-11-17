<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListQuestionSets.aspx.cs" Inherits="InitialRound.Web.Views.QuestionSet.ListQuestionSets" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<%@ Register Src="~/Controls/List.ascx" TagName="List" TagPrefix="controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/ListViewModel.js"></script>
    <script type="text/javascript" src="/Scripts/QuestionSet/ListQuestionSets.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new ListQuestionSets);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h2>Question Sets</h2>

    <div>
        <a class="btn" href="/QuestionSets/Create"><span class="icon-plus"></span> New Question Set</a>
    </div>

    <controls:List runat="server" />
</asp:Content>
