<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="InitialRound.Web.Views.Users.CreateUser"
    MasterPageFile="~/Site.master" %>

<%@ Import Namespace="InitialRound.BusinessLogic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Scripts/Users/CreateUser.js"></script>
    <script type="text/javascript">
        window.onload = function () {
            ko.applyBindings(new CreateUser("<%= AntiForgeryToken %>"));
        };
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="BodyContent">
    <h1>New User</h1>
    
    <p>
        All fields are required.
    </p>

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
            <label class="control-label" for="inputUsr">Desired Username</label>
            <div class="controls">
                <input id="inputUsr" type="text" data-bind="value: Username" />
            </div>
        </div>
        <div class="control-group">
            <label class="control-label" for="inputEa1">Email Address</label>
            <div class="controls">
                <input id="inputEa1" type="text" data-bind="value: EmailAddress1" />
                <p><small>Your name and email address will be included in emails sent by Initial Round on your behalf.</small></p>
            </div>
        </div>
        <div class="control-group">
            <label class="control-label" for="inputEa2">Confirm Email Address</label>
            <div class="controls">
                <input id="inputEa2" type="text" data-bind="value: EmailAddress2" />
            </div>
        </div>
        <div class="control-group">
            <label class="control-label" for="inputPwd1">Password</label>
            <div class="controls">
                <input id="inputPwd1" type="password" data-bind="value: Password1" />
            </div>
        </div>
        <div class="control-group">
            <label class="control-label" for="inputPwd2">Confirm Password</label>
            <div class="controls">
                <input id="inputPwd2" type="password" data-bind="value: Password2" />
            </div>
        </div>
        <div class="control-group">
            <div class="controls">
                <button type="submit" class="btn btn-primary" data-bind="click: CreateCommand">Create User</button>
            </div>
        </div>
    </div>

</asp:Content>
