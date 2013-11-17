ListApplicants.prototype = new ListViewModel();

function ListApplicants() {
    var self = this;
    self.ListViewModel(this);

    self.serviceAddress = "/services/applicants/GetApplicants";

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
            Header: 'Email Address',
            Property: 'EmailAddress',
            Filter: {
                Property: 'EmailAddress',
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

    self.DeleteURITemplate = "/Applicants/Delete/";
    self.DetailsURITemplate = "/Applicants/Details/";

    self.Name = ko.observable(null);
    self.EmailAddress = ko.observable(null);

    self.PopulateQuery = function (query) {
        query.Name = self.Name();
        query.EmailAddress = self.EmailAddress();
    };

    self.Name.subscribe(self.LoadResults);
    self.EmailAddress.subscribe(self.LoadResults);

    self.LoadResults();
}