<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CreateInterview.aspx.cs" Inherits="InitialRound.Web.Views.Interview.CreateInterview" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<%@ Register Src="~/Controls/QuestionSearch.ascx" TagName="QuestionSearch" TagPrefix="controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/QuestionSearch.js"></script>
    <script type="text/javascript" src="/Scripts/Interview/CreateInterview.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new CreateInterview('<%= AntiForgeryToken %>',
                '<%= Request["applicantId"] %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h1>Create Interview</h1>

    <h2>Basic Information</h2>

    <div class="form-horizontal">
        <div class="control-group">
            <label class="control-label" for="inputMa">Minutes Allowed</label>
            <div class="controls">
                <input id="inputMa" type="text" data-bind="value: MinutesAllowed" />
            </div>
        </div>
    </div>

    <hr />

    <h2>Questions</h2>

    <p class="text-info">
        Would you like to use an existing question set, or select questions manually?
    </p>

    <p>
        <label class="radio">
            <input type="radio" value="QuestionSet" name="InterviewType"
                data-bind="checked: Type" />
            Use Question Set</label>
        <label class="radio">
            <input type="radio" value="Questions" name="InterviewType" data-bind="checked: Type" />
            Select Questions</label>
    </p>

    <div data-bind="if: Type() === 'Questions'">
        <!-- ko with: QuestionSearch -->
        <controls:QuestionSearch runat="server" />
        <!-- /ko -->
    </div>

    <div data-bind="if: Type() === 'QuestionSet'">
        <p class="text-info">Please select a question set:</p>
        <p data-bind="foreach: QuestionSets">
            <label class="radio">
                <input type="radio" value="QuestionSet" name="QuestionSetID" data-bind="checked: $parent.QuestionSetID, value: ID, attr: { 'ID': 'QuestionSet-' + ID }" />
                <span data-bind="text: Name">Use Question Set</span>
            </label>
        </p>
        <p data-bind="visible: QuestionSets().length == 0" style="color: #808080">
            <em>No question sets found.</em>
        </p>
    </div>

    <p>
        <button type="submit" class="btn btn-primary" data-bind="click: CreateCommand">
            Create Interview</button>
    </p>
</asp:Content>
