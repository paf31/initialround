<%@ Page Title="" Language="C#" MasterPageFile="~/External.Master" AutoEventWireup="true" CodeBehind="Run.aspx.cs" Inherits="InitialRound.Web.Views.Interview.Run" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/Extensions.js"></script>
    <script type="text/javascript" src="/Scripts/Interview/Run.js"></script>
    <script type="text/javascript" src="/Scripts/Base/jquery.form.js"></script>

    <script type="text/javascript" src="/Scripts/Markdown/Markdown.Converter.js"></script>
    <script type="text/javascript" src="/Scripts/Markdown/Markdown.Sanitizer.js"></script>
    <script type="text/javascript" src="/Scripts/Markdown/Markdown-mvvm.js"></script>

    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new Interview('<%= Request["Token"] %>'));
            $(document.body).show();
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <div data-bind="if: Step() == 1">
        <h1>Welcome</h1>

        <h2>Instructions</h2>

        <style>
            .instructions li
            {
                margin-bottom: 5px;
            }
        </style>

        <ul class="instructions">
            <li>There are a total of <strong><span data-bind="text: NumberOfQuestions"></span>
                questions</strong>.</li>
            <li>You will have <strong><span data-bind="text: MinutesAllowed"></span>
                minutes</strong> to complete the questions.</li>
            <li>Each question contains a problem which must be solved by writing code.
                <br />
                For each question, you will be able to download a text file by clicking <strong>Download Input</strong>. This text file should be used as input to your program.
                <br />
                Your program should generate another text file as output, based on the problem description.
                <br />
                The output of your program will be uploaded and used to verify the correctness of your solutions.</li>
            <li>The format of your output file must be exactly correct. Be careful not to include additional spaces or lines in the generated output file.</li>
            <li>After you upload your output file, a set of tests will be run.<br />
                Use the results of the tests to find any bugs in your code, and try to make as many test cases pass as possible.<br />
                You may make multiple attempts at each question.</li>
            <li>Some questions may require you to download a new input file each time you submit a solution.<br />
                In this case, each generated input file will be different, so be sure to run your solution using the latest input file.</li>
            <li>You can solve the questions in any order, but no more solutions will be accepted after <strong><span data-bind="text: MinutesAllowed"></span>
                minutes</strong>.</li>
            <li>Each time you run a set of tests by clicking the <strong>Run Tests</strong> button, your output and code will be saved.<br />
                Your attempt will not be saved unless you click the <strong>Run Tests</strong> button.</li>
        </ul>

        <p>Use the <strong>Practice Environment</strong> to get used to the interview workflow without any time constraints.</p>

        <p>When you are ready to begin the interview, click <strong>Begin Interview</strong> below.</p>

        <p>
            <a class="btn btn-primary" href="/Interviews/Practice" target="_blank">Practice</a>
            <button type="button" class="btn btn-primary" data-bind="click: BeginInterviewCommand">Begin Interview</button>
        </p>
    </div>

    <div data-bind="if: Step() == 2">
        <div class="row-fluid">
            <div class="span2 bs-docs-sidebar">
                <h2>Questions</h2>

                <ul class="nav nav-list bs-docs-sidenav" data-bind="foreach: Questions">
                    <li data-bind="css: { 'active': $data === $root.SelectedQuestion() }">
                        <a href="#" data-bind="click: $parent.SelectQuestionCommand">
                            <i data-bind="css: { 'icon-ok' : Submitted }"></i>
                            <span data-bind="text: Name"></span>
                        </a>
                    </li>
                </ul>

                <p>&nbsp;</p>

                <p><strong>Time Remaining: </strong><span data-bind="text: TimeRemaining"></span></p>
            </div>
            <div class="span10">
                <h2>Description</h2>

                <p data-bind="markdown: SelectedQuestion() == null ? null : SelectedQuestion().QuestionBody"></p>

                <hr />

                <h3>Solution Upload</h3>

                <p>Select your output file and code file and click <strong>Upload and Run Tests</strong> to run the test suite.</p>

                <form id="pvForm">
                    <div class="form-horizontal">
                        <div class="control-group">
                            <label class="control-label" for="outputNm">Output</label>
                            <div class="controls">
                                <input id="pvOutput" name="Output" type="file" />
                            </div>
                            <label class="control-label" for="outputNm">Code</label>
                            <div class="controls">
                                <input id="pvCode" name="Code" type="file" />
                            </div>
                        </div>
                    </div>
                </form>

                <p>
                    <button type="button" class="btn" data-bind="click: DownloadInputCommand">
                        <span class="icon-download"></span>
                        Download Input</button>
                    <button type="button" class="btn" data-bind="click: RunTestsCommand, enable: SelectedQuestion() != null && SelectedQuestion().CanRunTests">
                        <span class="icon-upload"></span>
                        Run Tests</button>
                </p>

                <hr />

                <h3>Results</h3>

                <p>
                    <span data-bind="text: _.size(_.filter(Results(), function (x) { return x.Success; }))"></span>
                    of 
                    <span data-bind="text: _.size(Results())"></span>
                    tests passed
                </p>

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
                            <td style="font-family: monospace; word-wrap: break-word;" data-bind="text: Input"></td>
                            <td style="font-family: monospace; word-wrap: break-word;" data-bind="text: ExpectedOutput"></td>
                            <td style="font-family: monospace; word-wrap: break-word;" data-bind="text: Output"></td>
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
    </div>

    <div data-bind="if: Step() == 3">
        <h1>Thank You</h1>

        <p>The interview is complete. You may now close the browser window.</p>
    </div>
</asp:Content>
