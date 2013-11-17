<%@ Page Title="" Language="C#" MasterPageFile="~/External.Master" AutoEventWireup="true" CodeBehind="Practice.aspx.cs" Inherits="InitialRound.Web.Views.Interview.Practice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/Extensions.js"></script>
    <script type="text/javascript" src="/Scripts/Interview/Practice.js"></script>
    <script type="text/javascript" src="/Scripts/Base/jquery.form.js"></script>

    <script type="text/javascript" src="/Scripts/Markdown/Markdown.Converter.js"></script>
    <script type="text/javascript" src="/Scripts/Markdown/Markdown.Sanitizer.js"></script>
    <script type="text/javascript" src="/Scripts/Markdown/Markdown-mvvm.js"></script>

    <style>
        .instructions li
        {
            margin-bottom: 5px;
        }
    </style>

    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new Practice());
            $(document.body).show();
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <div class="row-fluid">
        <div class="span2 bs-docs-sidebar">
            <h2>Questions</h2>
            <div class="highlight" id="questionList">
                <ol class="nav nav-list bs-docs-sidenav nav">
                    <li class="active">
                        <a href="javascript: void(0);">
                            <i data-bind="css: { 'icon-ok' : $root.Submitted }"></i>
                            Calculating Squares
                        </a>
                    </li>
                </ol>
            </div>
        </div>
        <div class="span6">
            <div class="highlight" id="problemDescription">
                <h2>Description</h2>
                <div>
                    <p>Write a program which calculates the square of a number.</p>
                    <p>Each line of input will contain a single integer. For each line of input, output a single line containing the square of that integer.</p>
                    <p>Example input</p>
                    <pre>5
12</pre>
                    <p>Example output</p>
                    <pre>25
144</pre>
                </div>
            </div>

            <hr />

            <h3>Solution Upload</h3>

            <p>Select your output file and code file and click <strong>Upload and Run Tests</strong> to run the test suite.</p>

            <div class="highlight" id="uploadOutput">
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
            </div>

            <p>
                <button type="button" class="btn" data-bind="click: DownloadInputCommand">
                    <span class="icon-download"></span>
                    Download Input</button>
                <button type="button" class="btn" data-bind="click: RunTestsCommand">
                    <span class="icon-upload"></span>
                    Upload and Run Tests</button>
            </p>

            <hr />

            <h3>Results</h3>

            <p>
                <span data-bind="text: _.size(_.filter(Results(), function (x) { return x.Success; }))"></span>
                of 
                <span data-bind="text: _.size(Results())"></span>
                tests passed
            </p>

            <div class="highlight" id="testResults">
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
        <div class="span4">
            <h2>Practice Guide</h2>

            <div class="well">
                <p>Use this practice environment to get used to the interview workflow:</p>
                <ul class="instructions">
                    <li>Click on a question on the list on the left hand side to work on it.
                    </li>
                    <li>Read the problem description and implement the solution in your choice of programming language. See the <a href="#" data-bind="click: ShowExamplesCommand">list of example solutions</a>.
                    </li>
                    <li>Download the input text file and run your program using that file as input.
                    </li>
                    <li>Select your output file and code file.
                    </li>
                    <li>Run the tests. Your output and code files will be uploaded and validated.
                    </li>
                    <li>Inspect the test results and correct your code to address any failing test cases. Failed tests are marked in red. You can upload multiple attempts, but be sure to select the correct output and code files each time.
                    </li>
                </ul>
            </div>
        </div>
    </div>

    <div class="backdrop" data-bind="popup: ShowExamples">
        <div class="overlay container">
            <div class="modal-header">
                <h3>Example Solutions</h3>
            </div>
            <div class="modal-body">
                <ul class="nav nav-tabs" id="solutions">
                    <li class="active"><a href="javascript: void(0);" data-target="#java">Java</a></li>
                    <li><a href="javascript: void(0);" data-target="#csharp">C#</a></li>
                    <li><a href="javascript: void(0);" data-target="#python3">Python 3</a></li>
                </ul>
                <div class="tab-content">
                    <div class="tab-pane active" id="java">
                        <pre style="height: 200px; overflow-y: auto;">import java.io.*;
import java.util.*;
import org.apache.commons.io.*;

public class Program {
    public static void main(String[] args) {
        try {
            List&lt;String&gt; input = FileUtils.readLines(new File("input.txt"));
            List&lt;String&gt; output = new ArrayList&lt;String&gt;();
    
            StringBuilder builder = new StringBuilder();

            boolean first = true;

            for (String line : input) {
                if (!first) {   
                    builder.append("\n");
                }
                long n = Long.parseLong(line);
                long square = n * n;
                builder.append(Long.toString(square));
                first = false;
            }
    
            FileUtils.writeStringToFile(new File("output.txt"), builder.toString());
        } catch (java.io.IOException ex) {
        }
    }
}</pre>
                    </div>
                    <div class="tab-pane" id="csharp">
                        <pre style="height: 200px; overflow-y: auto;">using System.IO;
using System.Linq;

namespace Squares
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            var numbers = lines.Select(long.Parse);
            var squares = numbers.Select(n => n * n);
            File.WriteAllText("output.txt", string.Join("\n", squares));
        }
    }
}</pre>
                    </div>
                    <div class="tab-pane" id="python3">
                        <pre style="height: 200px; overflow-y: auto;">import sys

input = open('input.txt', 'r')
lines = input.readlines()
input.close()

ns = [ int(line) for line in lines ]
squares = [ n * n for n in ns ]

output = open('output.txt', 'w')
outputlines = [ str(square) for square in squares ]
output.writelines('\n'.join(outputlines))
output.close()</pre>
                    </div>
                </div>
                <script>
                    $(function () {
                        $('#solutions a').click(function (e) {
                            $(this).tab('show');
                        });
                        $('#solutions a:first').tab('show');
                    })
                </script>
            </div>
            <div class="modal-footer">
                <a href="#" class="btn btn-primary" data-bind="click: function() { ShowExamples(false); }">Close</a>
            </div>
        </div>
    </div>
</asp:Content>
