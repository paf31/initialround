<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Print.aspx.cs" Inherits="InitialRound.Web.Views.Interview.Print" %>

<%@ Import Namespace="InitialRound.BusinessLogic" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Print</title>
    <link href='https://fonts.googleapis.com/css?family=Lato' rel='stylesheet' type='text/css' />

    <script type="text/javascript" src="/Scripts/knockout-2.1.0.js"></script>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
    <script type="text/javascript" src="/Scripts/Base/cookies.js"></script>
    <script type="text/javascript" src="/Scripts/Base/ViewModel.js"></script>
    <script type="text/javascript" src="/Scripts/Interview/Print.js"></script>

    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new Print('<%= RouteData.Values["interviewId"] %>'));
            $(document.body).show();
        };
    </script>

    <style>
        body
        {
            font-family: 'Lato', sans-serif;
            display: none;
        }

        .underline
        {
            border-bottom: 1px solid #808080;
        }

        @media print
        {
            .indent
            {
                page-break-after: always;
            }
        }

        li
        {
            list-style-type: none;
        }

        pre
        {
            padding: 4px;
            border: 1px solid #D0D0D0;
        }

        table
        {
            border-collapse: collapse;
        }

        td
        {
            border: 1px solid #A0A0A0;
            padding: 3px;
        }

        thead td
        {
            font-weight: 700;
        }

        .indent
        {
            margin: 0 40px;
        }
    </style>
</head>
<body>
    <form runat="server">
        <div data-bind="visible: Busy">Loading...</div>

        <div style="color: red;" data-bind="text: ErrorMessage"></div>

        <div data-bind="visible: !Busy()">
            <h1 style="border-bottom: 3px double #808080;">
                <img src="/Images/logo-black.png" />
                Initial Round
            </h1>

            <p>Applicant Name: <span data-bind="text: Response() ? Response().ApplicantName ? 'No Name' : Response().ApplicantName : null"></span></p>

            <p><span data-bind="text: Response() ? Response().PercentageOfPartiallyCorrectQuestions : null"></span>% Partially Correct Questions</p>

            <p><span data-bind="text: Response() ? Response().PercentageOfPerfectQuestions : null"></span>% Completely Correct Questions</p>

            <p><span data-bind="text: Response() ? Response().PercentageOfTestsPassed : null"></span>% Attempted Tests Passed</p>

            <div data-bind="foreach: Response() ? Response().Questions : null">
                <h2 class="underline" data-bind="text: Name"></h2>
                <div class="indent">
                    <p><span data-bind="text: PercentageOfTestsPassed"></span>% Tests Passed</p>
                    <h3 class="underline">Test Results</h3>
                    <table style="table-layout: fixed; width: 100%;">
                        <thead>
                            <tr>
                                <td>Name</td>
                                <td>Result</td>
                                <td>Input</td>
                                <td>Expected Output</td>
                                <td>Output</td>
                            </tr>
                        </thead>
                        <tbody data-bind="foreach: TestResults">
                            <tr>
                                <td data-bind="text: TestName"></td>
                                <td data-bind="text: Success ? 'Passed' : 'Failed'"></td>
                                <td data-bind="text: Input" style="font-family: monospace; word-wrap: break-word;"></td>
                                <td data-bind="text: ExpectedOutput" style="font-family: monospace; word-wrap: break-word;"></td>
                                <td data-bind="text: Output" style="font-family: monospace; word-wrap: break-word;"></td>
                            </tr>
                        </tbody>
                    </table>

                    <h3 class="underline">Code</h3>

                    <pre data-bind="text: Code" style="word-wrap: break-word;"></pre>

                    <h3 class="underline">Output</h3>

                    <pre data-bind="text: Output" style="word-wrap: break-word;"></pre>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
