<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionSearch.ascx.cs" Inherits="InitialRound.Web.Controls.QuestionSearch" %>

<p data-bind="foreach: SelectedQuestions.sortBy(function(q) { return q.Name; })">
    <a class="btn btn-primary" href="#" data-bind="click: $parent.RemoveQuestionCommand">
        <i class="icon-remove icon-white"></i>
        <span data-bind="text: Name"></span>
    </a>
</p>

<p data-bind="visible: SelectedQuestions().length == 0" style="color: #808080"><em>There are no questions selected</em></p>

<div>
    <label for="questionSearch">Search for Questions:</label><div class="input-append">
    <input id="questionSearch" type="text" data-bind="value: QuestionNameFilter, valueUpdate: 'keyup'" />
    <button class="btn" type="button" data-bind="click: ShowAllQuestions">Remove Filter</button></div>
</div>


<p data-bind="foreach: SearchResults">
    <a class="btn" href="#" data-bind="click: $parent.AddQuestionCommand, enable: $parent.CanAddQuestion($parent, $data), visible: $parent.QuestionIsNotSelected($parent, $data)">
        <i class="icon-plus"></i>
        <span data-bind="text: Name"></span>
    </a>
</p>
