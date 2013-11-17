<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="QuestionSet.aspx.cs" Inherits="InitialRound.Web.Views.QuestionSet.QuestionSet" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<%@ Register Src="~/Controls/QuestionSearch.ascx" TagName="QuestionSearch" TagPrefix="controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/QuestionSearch.js"></script>
    <script type="text/javascript" src="/Scripts/QuestionSet/QuestionSet.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new QuestionSet('<%= AntiForgeryToken %>',
                <%= RouteData.Values.ContainsKey("questionSetId") ? "'" + RouteData.Values["questionSetId"] + "'" : "null" %>));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h1>Question Set</h1>

    <div class="form-horizontal">
        <div class="control-group">
            <label class="control-label" for="inputName">Name</label>
            <div class="controls">
                <input id="inputName" type="text" data-bind="value: Name" />
            </div>
        </div>
    </div>

    <h2 class="legend">Questions</h2>

    <!-- ko with: QuestionSearch -->
    <controls:QuestionSearch runat="server" />
    <!-- /ko -->

    <div>
        <button type="submit" class="btn btn-primary" data-bind="visible: QuestionSetID() == null, click: CreateCommand">Create Question Set</button>
        <button type="submit" class="btn btn-primary" data-bind="visible: QuestionSetID() != null, click: SaveCommand">Save Changes</button>
    </div>
</asp:Content>
