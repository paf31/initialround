<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="InitialRound.Web.Views.Help" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new ViewModel());
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h1>Getting Started Guide</h1>

    <a id="HowItWorks"></a>
    <h3>How It Works</h3>
    <p>An interview consists of a number of questions. Before undertaking each question, the applicant will be asked to download a text file, which consists of a list of lines of input. The applicant will then be asked to write a program to solve some particular task for each input value, and output a list of lines of output to a second text file.</p>
    <p>Outputs in the output text file will be compared to expected outputs, and saved as the test results. The applicant will be shown which tests passed and which ones failed, and will be able to make multiple attempts in order to make as many test cases pass as possible. Each attempt will be recorded for reporting purposes.</p>
    <p>Try to include an example input and output pair in the problem description if appropriate.</p>

    <a id="WritingProblemDescriptions"></a>
    <h3>Writing Problem Descriptions</h3>
    <p>Problem descriptions are written using the <em>Markdown</em> formatting language. This allows basic formatting of text including the use of bold, italic and underlined text, section headers, ordered and unordered lists, code samples and blockquotes.</p>
    <p>Try to describe the problem succinctly, describing any border cases and restrictions which the applicant can reasonably assume.</p>
    
    <a id="WritingTests"></a>
    <h3>Writing Tests</h3>
    <p>Tests are specified as pairs of lines of input and expected output. For each test, either the applicant's output is exactly correct or it is marked as incorrect, so try to be very specific about the expected format of the output in your problem descriptions.</p>

    <a id="ExampleQuestion"></a>
    <h3>Example Question</h3>
    <p>A simple question might ask the user to write a program to sum a list of integers.</p>
    <p>The problem description might read as follows:</p>
    <pre>Write a program to compute the sum of a list of positive or negative integers. Each test case will consist of a comma separated list of integers. For each test case, output the sum of the integers in the list.<br /><br />Example input: "1,3,-2,4,5"<br />Example output: "11"<br /><br />Restrictions: each input list will consist of < 100 integers in the range -1000000 to +1000000.</pre>

</asp:Content>
