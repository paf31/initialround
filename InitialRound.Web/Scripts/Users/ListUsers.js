ListUsers.prototype = new ListViewModel();

function ListUsers() {
    var self = this;
    self.ListViewModel(this);

    self.Columns = [
        {
            Header: 'Username',
            Property: 'Username',
            Filter: {
                Property: 'Username',
                Type: 'text'
            }
        },
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
            Header: 'Last Updated On',
            Property: 'LastUpdatedDate',
            Filter: null
        },
        {
            Header: 'Created On',
            Property: 'CreatedDate',
            Filter: null
        }
    ];

    self.PrimaryKey = 'Username';
    self.DeleteURITemplate = "/Users/Delete/";
    self.DetailsURITemplate = "/Users/Details/";

    self.serviceAddress = "/services/users/ListUsers";

    self.Name = ko.observable(null);
    self.Username = ko.observable(null);
    self.EmailAddress = ko.observable(null);

    self.PopulateQuery = function (query) {
        query.Name = self.Name();
        query.EmailAddress = self.EmailAddress();
        query.Username = self.Username();
    };

    self.Name.subscribe(self.LoadResults);
    self.EmailAddress.subscribe(self.LoadResults);
    self.Username.subscribe(self.LoadResults);

    self.LoadResults();
}