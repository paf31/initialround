ListInterviews.prototype = new ListViewModel();

function ListInterviews(statusId) {
    var self = this;
    self.ListViewModel(this);

    self.serviceAddress = "/services/interviews/GetInterviews";

    self.Name = ko.observable(null);
    self.Status = ko.observable(statusId);

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
            Header: 'Status',
            Property: 'Status',
            Filter: {
                Property: 'Status',
                Type: 'select',
                Options: [
                    { Text: 'Any', Value: null },
                    { Text: 'Created', Value: 0 },
                    { Text: 'Invitation Sent', Value: 1 },
                    { Text: 'In Progress', Value: 2 },
                    { Text: 'Completed', Value: 3 },
                ]}
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

    self.DeleteURITemplate = "/Interviews/Delete/";
    self.DetailsURITemplate = "/Interviews/Details/";

    self.PopulateQuery = function (query) {
        query.Name = self.Name();
        query.Status = self.Status();
    };

    self.Name.subscribe(self.LoadResults);
    self.Status.subscribe(self.LoadResults);

    self.LoadResults();
}