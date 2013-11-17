ListQuestionSets.prototype = new ListViewModel();

function ListQuestionSets() {
    var self = this;
    self.ListViewModel(this);

    self.Columns = [
        {
            Header: 'Name',
            Property: 'Name',
            Filter: {
                Property: 'Name',
                Type: 'text'
            }
        },
        {
            Header: 'Last Updated By',
            Property: 'LastUpdatedBy',
            Filter: null
        },
        {
            Header: 'Last Updated On',
            Property: 'LastUpdatedDate',
            Filter: null
        },
        {
            Header: 'Created By',
            Property: 'CreatedBy',
            Filter: null
        },
        {
            Header: 'Created On',
            Property: 'CreatedDate',
            Filter: null
        }
    ];

    self.DeleteURITemplate = "/QuestionSets/Delete/";
    self.DetailsURITemplate = "/QuestionSets/Details/";

    self.serviceAddress = "/services/questionsets/GetQuestionSets";

    self.Name = ko.observable(null);

    self.PopulateQuery = function (query) {
        query.Name = self.Name();
    };

    self.Name.subscribe(self.LoadResults);

    self.LoadResults();
}