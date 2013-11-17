<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EditUser.aspx.cs" Inherits="InitialRound.Web.Views.Users.EditUser" %>

<%@ Import Namespace="InitialRound.Models.Schema.dbo" %>
<%@ Import Namespace="InitialRound.BusinessLogic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Users/EditUser.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new EditUser('<%= AntiForgeryToken %>', '<%= RouteData.Values["username"] %>'));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">

    <h1>Edit User</h1>

    <div class="form-horizontal">
        <div class="control-group">
            <label class="control-label" for="inputUsr">Username</label>
            <div class="controls">
                <span class="uneditable-input" data-bind="text: username"></span>
            </div>
        </div>
        <div class="control-group">
            <label class="control-label" for="inputFn">First Name</label>
            <div class="controls">
                <input id="inputFn" type="text" data-bind="value: FirstName" />
            </div>
        </div>
        <div class="control-group">
            <label class="control-label">Last Name</label>
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
                <button type="submit" class="btn btn-primary" data-bind="click: SaveCommand">Save Changes</button>
            </div>
        </div>
    </div>
</asp:Content>
