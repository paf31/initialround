<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="List.ascx.cs" Inherits="InitialRound.Web.Controls.List" %>

<div style="overflow-x: auto;">
    <table class="table table-striped">
        <thead>
            <tr>
                <td></td>
                <!-- ko foreach: Columns -->
                <td>
                    <label style="font-weight: bold;" data-bind="text: Header, attr: { 'for': 'filter' + Header }"></label>
                    <!-- ko if: Filter != null -->
                    <!-- ko if: Filter.Type == 'text' -->
                    <input type="text" class="search-query" data-bind="value: $parent[$data.Filter.Property], attr: { 'id': 'filter' + Header }" placeholder="Search" />
                    <!-- /ko -->
                    <!-- ko if: Filter.Type == 'select' -->
                    <select data-bind="value: $parent[$data.Filter.Property], options: Filter.Options, optionsText: 'Text', optionsValue: 'Value', attr: { 'id': 'filter' + Header }"></select>
                    <!-- /ko -->
                    <!-- /ko -->
                </td>
                <!-- /ko -->
            </tr>
        </thead>
        <tbody data-bind="foreach: Results">
            <tr>
                <td class="span1">
                    <a title="Delete" data-bind='attr: { href: $parent.DeleteURITemplate + $data[$parent.PrimaryKey] }' class="btn btn-small"><span class="icon-trash"></span></a>
                </td>
                <!-- ko foreach: $parent.Columns -->
                <!-- ko if: $index() > 0 -->
                <td data-bind="text: $parent[$data.Property]"></td>
                <!-- /ko -->
                <!-- ko if: $index() == 0 -->
                <td>
                    <a data-bind="attr: { href: $parentContext.$parent.DetailsURITemplate + $parent[$parentContext.$parent.PrimaryKey] }, text: $parent[$data.Property]"></a>
                </td>
                <!-- /ko -->
                <!-- /ko -->
            </tr>
        </tbody>
        <tbody data-bind="visible: !_.any(Results())">
            <tr>
                <td class="span1" data-bind="attr: { 'colspan': 2 + Columns.length }" style="text-align: center;"><em style="color: #808080; padding: 5px;">No data</em></td>
            </tr>
        </tbody>
    </table>
</div>

<p>
    Displaying <span
        data-bind="text: 1 + (PageNumber() - 1) * <%= InitialRound.Web.Properties.Settings.Default.PageSize %>"></span>-<span
            data-bind="text: Math.min(PageNumber() * <%= InitialRound.Web.Properties.Settings.Default.PageSize %>, TotalCount())"></span> of <span
                data-bind="text: TotalCount"></span>
</p>

<div class="btn-group">
    <button class="btn" data-bind="click: PrevPageCommand, enable: PageNumber() > 1"><span class="icon-chevron-left"></span>Prev Page</button>
    <button class="btn" data-bind="click: NextPageCommand, enable: PageNumber() * <%= InitialRound.Web.Properties.Settings.Default.PageSize %> < TotalCount()">Next Page <span class="icon-chevron-right"></span></button>
</div>
