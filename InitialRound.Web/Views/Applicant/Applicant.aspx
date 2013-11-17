<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Applicant.aspx.cs" Inherits="InitialRound.Web.Views.Applicant.Applicant" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<%@ Register Src="~/Controls/List.ascx" TagName="List" TagPrefix="controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/ListViewModel.js"></script>
    <script type="text/javascript" src="/Scripts/Applicant/Applicant.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new Applicant('<%= AntiForgeryToken %>',
                <%= RouteData.Values.ContainsKey("applicantId") ? "'" + RouteData.Values["applicantId"] + "'" : "null" %>));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h1>Applicant</h1>

    <h2>Basic Information</h2>

    <div class="form-horizontal">
        <div class="control-group">
            <label class="control-label" for="inputFn">First Name</label>
            <div class="controls">
                <input id="inputFn" type="text" data-bind="value: FirstName" />
            </div>
        </div>
        <div class="control-group">
            <label class="control-label" for="inputLn">Last Name</label>
            <div class="controls">
                <input id="inputLn" type="text" data-bind="value: LastName" />
            </div>
        </div>
        <div class="control-group">
            <label class="control-label" for="inputEa">Email Address</label>
            <div class="controls">
                <input id="inputEa" type="text" data-bind="value: EmailAddress" />
            </div>
        </div>
        <div class="control-group">
            <div class="controls">
                <button data-bind="visible: ApplicantID() == null, click: CreateCommand" type="submit" class="btn btn-primary">Create Applicant</button>
                <button data-bind="visible: ApplicantID() != null, click: SaveCommand" type="submit" class="btn btn-primary">Save Changes</button>
            </div>
        </div>
    </div>

    <div data-bind="if: ApplicantID() != null">
        <h2>Interviews</h2>

        <div>
            <a class="btn" data-bind="attr: { 'href': '/Interviews/Create?ApplicantID=' + ApplicantID() }"><span class="icon-plus"></span> New Interview</a>
        </div>

        <!-- ko with: ApplicantInterviews -->
        <controls:List runat="server" />
        <!-- /ko -->
    </div>
</asp:Content>
