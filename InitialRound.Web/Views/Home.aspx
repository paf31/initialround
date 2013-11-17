<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Home.aspx.cs" Inherits="InitialRound.Web.Views.Home" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="/Scripts/Home.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new Home('<%= AntiForgeryToken %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="BodyContent">
    <h1>Dashboard</h1>

    <div style="overflow-x: auto;">
        <table class="table table-striped">
            <tr>
                <td class="lead span2"><a href="/Interviews?Status=0" data-bind="text: NewInterviews"></a></td>
                <td>New Interviews</td>
            </tr>
            <tr>
                <td class="lead span2"><a href="/Interviews?Status=1" data-bind="text: PendingInterviews"></a></td>
                <td>Pending Interviews</td>
            </tr>
            <tr>
                <td class="lead span2"><a href="/Interviews?Status=2" data-bind="text: InterviewsInProgress"></a></td>
                <td>Interviews In Progress</td>
            </tr>
            <tr>
                <td class="lead span2"><a href="/Interviews?Status=3" data-bind="text: InterviewsRequiringReview"></a></td>
                <td>Interviews Requiring Review</td>
            </tr>
        </table>
    </div>

    <h2>Quick Start</h2>

    <div class="alert alert-warning" data-bind="visible: QuestionSets() != null && !_.any(QuestionSets())">
        You have not defined any question sets. <a href="/QuestionSets/Create">Create a question set</a> now to enable the Quick Start feature.
    </div>

    <div class="form-horizontal" data-bind="visible: _.any(QuestionSets())">
        <p>Enter an applicant's email address and select a question set to create an interview in a single step:</p>

        <div class="control-group">
            <label class="control-label" for="inputEmail">Email Address</label>
            <div class="controls">
                <input id="inputEmail" type="text" data-bind="value: EmailAddress" />
            </div>
        </div>
        <div class="control-group">
            <label class="control-label" for="inputQs">Question Set</label>
            <div class="controls">
                <select id="inputQs" data-bind="value: QuestionSetID, options: QuestionSets, optionsText: 'Name', optionsValue: 'ID', optionsCaption: 'Select'"></select>
            </div>
        </div>
        <div class="control-group">
            <label class="control-label" for="inputMa">Minutes Allowed</label>
            <div class="controls">
                <input id="inputMa" type="text" data-bind="value: MinutesAllowed" />
            </div>
        </div>
        <div class="control-group">
            <div class="controls">
                <label class="checkbox">
                    Send Invitation Email Now
                    <input type="checkbox" data-bind="checked: SendInvitation" />
                </label>
                <div>
                    <small>(Your name and email address will appear in the invitation email, and you will be CC'd.)</small>
                </div>
            </div>
        </div>
        <div class="control-group">
            <div class="controls">
                <button type="submit" class="btn" data-bind="click: QuickStartCommand">Create Interview</button>
            </div>
        </div>
    </div>

</asp:Content>
