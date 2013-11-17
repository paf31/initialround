ListQuestions.prototype = new ListViewModel();

function ListQuestions() {
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

    self.DeleteURITemplate = "/Questions/Delete/";
    self.DetailsURITemplate = "/Questions/Details/";

    self.serviceAddress = "/services/questions/GetQuestions";

    self.Name = ko.observable(null);

    self.PopulateQuery = function (query) {
        query.Name = self.Name();
    };

    self.Name.subscribe(self.LoadResults);

    self.LoadResults();
}