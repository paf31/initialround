<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Question.aspx.cs" Inherits="InitialRound.Web.Views.Question.Question" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/ListViewModel.js"></script>
    <script type="text/javascript" src="/Scripts/Question/Question.js"></script>
    <script type="text/javascript" src="/Scripts/Base/hideable.js"></script>
    <script type="text/javascript" src="/Scripts/Base/jquery.form.js"></script>
    <script type="text/javascript" src="/Scripts/Markdown/Markdown.Converter.js"></script>
    <script type="text/javascript" src="/Scripts/Markdown/Markdown.Sanitizer.js"></script>
    <script type="text/javascript" src="/Scripts/Markdown/Markdown-mvvm.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new Question('<%= AntiForgeryToken %>',
                <%= RouteData.Values.ContainsKey("questionId") ? "'" + RouteData.Values["questionId"] + "'" : "null" %>));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h1>Question</h1>

    <style>
        .nav-tabs li a
        {
            cursor: pointer;
        }
    </style>

    <ul class="nav nav-tabs">
        <li><a data-bind="click: function() { SelectedTab(0); }" data-toggle="tab" id="generalTab">General</a></li>
        <!-- ko ifnot: IsCodedTest -->
        <li><a data-bind="click: function() { SelectedTab(1); }" data-toggle="tab">Tests</a></li>
        <!-- /ko -->
        <li><a data-bind="click: function() { SelectedTab(2); }" data-toggle="tab">Preview</a></li>
    </ul>

    <script type="text/javascript">
        $(function() {
            $('#generalTab').tab('show');   
        });
    </script>

    <div data-bind="visible: SelectedTab() === 0">
        <div class="form-horizontal">
            <div class="control-group">
                <label class="control-label" for="inputNm">Name</label>
                <div class="controls">
                    <input id="inputNm" type="text" data-bind="value: Name, enable: CanEdit" />
                </div>
            </div>
        </div>

        <p class="muted"><small>The description is written using the Markdown language. It is shown to the applicant and should be used to describe the problem and any additional information about the test suite.</small></p>

        <h3>Description</h3>

        <div>
            <div class="btn-toolbar">
                <div class="btn-group">
                    <button class="btn btn-small" data-bind="wrap: '**', target: '#markdownEditor', targetProperty: 'QuestionBody'"
                        title="Bold">
                        <span class="icon-bold"></span>
                    </button>
                    <button class="btn btn-small" data-bind="wrap: '*', target: '#markdownEditor', targetProperty: 'QuestionBody'"
                        title="Italic">
                        <span class="icon-italic"></span>
                    </button>
                </div>
                <div class="btn-group">
                    <button class="btn btn-small" data-bind="prepend: '# ', target: '#markdownEditor', targetProperty: 'QuestionBody'"
                        title="Header">
                        <span class="icon-font"></span>
                    </button>
                </div>
                <div class="btn-group">
                    <button class="btn btn-small" data-bind="prepend: ' - ', target: '#markdownEditor', targetProperty: 'QuestionBody'"
                        title="List">
                        <span class="icon-list"></span>
                    </button>
                    <button class="btn btn-small" data-bind="prepend: '> ', target: '#markdownEditor', targetProperty: 'QuestionBody'"
                        title="Blockquote">
                        ><span class="icon-indent-left"></span></button>
                    <button class="btn btn-small" data-bind="prepend: '    ', target: '#markdownEditor', targetProperty: 'QuestionBody'"
                        title="Code">
                        <span class="icon-pencil"></span>
                    </button>
                    <button class="btn btn-small" data-bind="wrap: '`', target: '#markdownEditor', targetProperty: 'QuestionBody'"
                        title="Inline Code">
                        <span style="font-family: monospace; font-size: 0.8em; line-height: 1em;">&lt;/&gt;</span></button>
                </div>
            </div>
            <p>
                <textarea id="markdownEditor" data-bind="value: QuestionBody, enable: CanEdit"
                    style="width: 100%; height: 220px; font-family: monospace;"></textarea>
            </p>
        </div>

        <h3>Preview</h3>

        <p data-bind="markdown: QuestionBody"></p>
        <p data-bind="visible: QuestionBody() == null || QuestionBody() == ''"><em>No text</em></p>
    </div>

    <div data-bind="visible: SelectedTab() === 1">
        <p class="muted"><small>You can create multiple tests to measure an applicant's solution in different ways. The applicant will be given a random selection of the tests below.</small></p>

        <div style="overflow-x: auto;">
            <table class="table table-striped">
                <thead>
                    <tr>
                        <td class="span1"></td>
                        <td class="span3"><strong>Name</strong></td>
                        <td class="span3"><strong>Input</strong></td>
                        <td><strong>Expected Output</strong></td>
                    </tr>
                </thead>
                <tbody data-bind="foreach: Tests">
                    <tr>
                        <td class="span1"><a href="#" data-bind="click: $root.RemoveTestCommand, enable: $parent.CanEdit" class="btn btn-small"><span class="icon-trash"></span></a></td>
                        <td><input type="text" data-bind="value: Name, enable: $parent.CanEdit" /></td>
                        <td><input type="text" data-bind="value: Input, enable: $parent.CanEdit" /></td>
                        <td><input type="text" data-bind="value: ExpectedOutput, enable: $parent.CanEdit" /></td>
                    </tr>
                </tbody>
                <tbody data-bind="visible: !_.any(Tests())">
                    <tr>
                        <td class="span1"></td>
                        <td colspan="3"><em style="color: #808080; padding: 5px;">No data</em></td>
                    </tr>
                </tbody>
            </table>
        </div>

        <p>
            <button type="button" class="btn" data-bind="click: AddTestCommand, enable: CanEdit()">
                <span class="icon-plus"></span>
                New Test
            </button>
        </p>
        
        <h3>Import CSV</h3>

        <p class="muted"><small>Import tests in bulk by providing a CSV file with no headers and data separated into three columns: Name, Input and Expected Output.</small></p>
        
        <form id="csvForm">
            <input id="csvFile" name="CSV" type="file" />
        </form>
          
        <p><button type="submit" class="btn" data-bind="click: UploadCSVTestsCommand"><span class="icon-file"></span> Import</button></p>

    </div>

    <div data-bind="visible: SelectedTab() === 2">
        <p class="muted"><small>Test your question by entering a solution and running your tests. This is similar to how the applicant will see the question during their interview. This code will not be saved.</small></p>

        <div class="form-horizontal">
            <div class="control-group">
                <label class="control-label" for="outputNm">Output</label>
                <div class="controls">
                    <form id="pvForm">
                        <input id="pvOutput" name="Output" type="file" />
                    </form>
                </div>
            </div>
        </div>

        <div>
            <button type="button" class="btn" data-bind="click: DownloadInputCommand"><span class="icon-file"></span> Download Input</button>
            <button type="button" class="btn" data-bind="click: RunTestsCommand"><i class="icon-play"></i> Run Tests</button>
        </div>

        <h4>Test Results</h4>

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
                <tbody data-bind="foreach: PreviewResults">
                    <tr>
                        <td style="width: 30px;"><i data-bind="css: { 'icon-ok': Success, 'icon-remove': !Success }"></i></td>
                        <td data-bind="text: TestName"></td>
                        <td data-bind="text: Success ? 'Passed' : 'Failed'"></td>
                        <td style="font-family: monospace; word-wrap: break-word;" data-bind="text: Input"></td>
                        <td style="font-family: monospace; word-wrap: break-word;" data-bind="text: ExpectedOutput"></td>
                        <td style="font-family: monospace; word-wrap: break-word;" data-bind="text: Output"></td>
                    </tr>
                </tbody>
                <tbody data-bind="visible: !_.any(PreviewResults())">
                    <tr>
                        <td style="width: 30px;"></td>
                        <td colspan="5"><em style="color: #808080; padding: 5px;">No data</em></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <div>
        <!-- ko ifnot: QuestionID -->
        <button type="submit" class="btn btn-primary" data-bind="click: CreateCommand">Create Question</button>
        <!-- /ko -->
        <!-- ko if: QuestionID -->
        <!-- ko if: CanEdit -->
        <button type="submit" class="btn btn-primary" data-bind="click: SaveCommand">Save Changes</button>
        <!-- /ko -->
        <!-- ko ifnot: IsCodedTest --><!-- ko ifnot: CanEdit -->
        <button type="submit" class="btn" data-bind="click: CopyCommand">Copy Question</button>
        <!-- /ko --><!-- /ko -->
        <!-- /ko -->
    </div>
</asp:Content>
