<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Interview.aspx.cs" Inherits="InitialRound.Web.Views.Interview.Interview" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<%@ Register Src="~/Controls/QuestionSearch.ascx" TagName="QuestionSearch" TagPrefix="controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/QuestionSearch.js"></script>
    <script type="text/javascript" src="/Scripts/Interview/Interview.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new Interview('<%= AntiForgeryToken %>',
                '<%= RouteData.Values["interviewId"] %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h1>Interview
    </h1>

    <p data-bind="visible: Status() == 0 || Status() == 1">
        <button type="button" class="btn btn-primary" data-bind="click: SendInvitationCommand">
            <span class="icon-envelope icon-white"></span>
            <span data-bind="text: Status() == 0 ? 'Send Invitation' : 'Resend Invitation'"></span>
        </button>
        <br />
        <small>(Your name and email address will appear in the invitation email, and you will be CC'd.)</small>
    </p>

    <div class="row-fluid">
        <div class="span2">
            <strong>Status</strong>
        </div>
        <div class="span2" data-bind="text: StatusText"></div>
    </div>
    <div class="row-fluid">
        <div class="span2">
            <strong>Applicant</strong>
        </div>
        <div class="span2">
            <a data-bind="attr: { 'href': '/Applicants/Details/' + ApplicantID() }, text: /\S/.test(ApplicantName()) ? ApplicantName() : 'No Name'"></a>
        </div>
    </div>

    <div class="row-fluid" data-bind="visible: Status() > 1">
        <div class="span2">
            <strong>Time Remaining</strong>
        </div>
        <div class="span2" data-bind="text: TimeRemaining"></div>
    </div>

    <hr />

    <div data-bind="visible: Status() <= 1">
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
    </div>

    <div data-bind="visible: Status() <= 1">
        <h2>Questions</h2>
        <!-- ko with: QuestionSearch -->
        <controls:QuestionSearch runat="server" />
        <!-- /ko -->
    </div>
    <p data-bind="visible: Status() <= 1">
        <button type="submit" class="btn btn-primary" data-bind="click: SaveCommand">
            Save Changes</button>
    </p>

    <div class="section" data-bind="visible: Status() >= 2">
        <div class="row-fluid">
            <div class="span3 bs-docs-sidebar">
                <h3>Questions</h3>

                <ul class="nav nav-list bs-docs-sidenav" data-bind="foreach: Questions">
                    <li data-bind="css: { 'active': $data === $root.SelectedQuestion() }">
                        <a href="#" data-bind="click: $root.SelectQuestionCommand">
                            <i data-bind="css: { 'icon-chevron-right': $data !== $root.SelectedQuestion(), 'icon-chevron-down': $data === $root.SelectedQuestion()}"></i>
                            <span data-bind="text: Question.Name"></span>
                        </a>
                        <ul class="nav nav-list bs-docs-sidenav" data-bind="foreach: Attempts, visible: $data === $root.SelectedQuestion()">
                            <li data-bind="css: { 'active': $data === $root.SelectedAttempt() }">
                                <a href="#" data-bind="click: $root.LoadAttemptCommand, text: TimeOffset"></a>
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>

            <script type="text/javascript">
                $(function () {
                    $('.solutions a').on('click', function (e) {
                        $(this).tab('show');
                    });
                    $('.solutions a:first').tab('show');
                })
            </script>

            <div class="span9">
                <ul class="nav nav-tabs solutions">
                    <li class="active">
                        <a href="javascript: void(0);" data-target="#testResults">Test Results</a>
                    </li>
                    <li>
                        <a href="javascript: void(0);" data-target="#code">Code</a>
                    </li>
                    <li>
                        <a href="javascript: void(0);" data-target="#output">Output</a>
                    </li>
                </ul>
                <div class="tab-content">
                    <div class="tab-pane active" id="testResults" data-bind="with: SelectedAttempt">
                        <div>
                            <table class="table table-striped" style="table-layout: fixed; width: 100%;">
                                <thead>
                                    <tr>
                                        <td style="width: 30px;"></td>
                                        <td><strong>Name</strong></td>
                                        <td><strong>Status</strong></td>
                                        <td><strong>Input</strong></td>
                                        <td><strong>Expected Output</strong></td>
                                        <td><strong>Output</strong></td>
                                    </tr>
                                </thead>
                                <tbody data-bind="foreach: Results">
                                    <tr>
                                        <td style="width: 30px;"><i data-bind="css: { 'icon-ok': Success, 'icon-remove': !Success }"></i></td>
                                        <td data-bind="text: TestName"></td>
                                        <td data-bind="text: Success ? 'Passed' : 'Failed'"></td>
                                        <td data-bind="text: Input" style="font-family: monospace; word-wrap: break-word;"></td>
                                        <td data-bind="text: ExpectedOutput" style="font-family: monospace; word-wrap: break-word;"></td>
                                        <td data-bind="text: Output" style="font-family: monospace; word-wrap: break-word;"></td>
                                    </tr>
                                </tbody>
                                <tbody data-bind="visible: !_.any(Results())">
                                    <tr>
                                        <td style="width: 30px;"></td>
                                        <td colspan="5"><em style="color: #808080; padding: 5px;">No data</em></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="tab-pane" id="code" data-bind="with: SelectedAttempt">
                        <pre data-bind="text: Code" style="word-wrap: break-word;"></pre>
                    </div>
                    <div class="tab-pane" id="output" data-bind="with: SelectedAttempt">
                        <pre data-bind="text: Output" style="word-wrap: break-word;"></pre>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
