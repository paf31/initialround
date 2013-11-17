<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="BulkImport.aspx.cs" Inherits="InitialRound.Web.Views.Applicant.BulkImport" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Base/ListViewModel.js"></script>
    <script type="text/javascript" src="/Scripts/Applicant/BulkImport.js"></script>
    <script type="text/javascript" src="/Scripts/Base/jquery.form.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new BulkImport('<%= AntiForgeryToken %>', '<%= RouteData.Values["applicantId"] %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <h2>Bulk Import Applicants</h2>

    <p>Upload a CSV file containing the following columns (without headers) to create applicants in bulk:</p>

    <ul>
        <li>First Name</li>
        <li>Last Name</li>
        <li>Email Address</li>
    </ul>

    <form id="csvForm">
        <input id="csvFile" name="CSV" type="file" />
    </form>

    <p>
        <button type="submit" class="btn" data-bind="click: UploadCSVApplicantsCommand"><span class="icon-file"></span>Import</button>
    </p>

    <h3>Imported Rows</h3>

    <table class="table table-striped">
        <thead>
            <tr>
                <th>First Name</th>
                <th>Last Name</th>
                <th>Email Address</th>
            </tr>
        </thead>
        <tbody data-bind="foreach: Applicants">
            <tr>
                <td data-bind="text: FirstName"></td>
                <td data-bind="text: LastName"></td>
                <td data-bind="text: EmailAddress"></td>
            </tr>
        </tbody>
        <tbody data-bind="visible: !_.any(Applicants())">
            <tr>
                <td colspan="3"><em style="color: #808080; padding: 5px;">No data</em></td>
            </tr>
        </tbody>
    </table>

    <p>
        <button type="submit" class="btn btn-primary" data-bind="click: CreateApplicantsCommand, enable: _.any(Applicants())">Create</button>
    </p>
</asp:Content>
