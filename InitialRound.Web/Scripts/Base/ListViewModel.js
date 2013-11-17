ListViewModel.prototype = new ViewModel();

function ListViewModel() {
    var self = this;

    self.ListViewModel = function () {
        self.self = this;
        self.ViewModel(this);
    };

    self.PrimaryKey = 'ID';

    self.PageNumber = ko.observable(1);

    self.Results = ko.observableArray([]);
    self.TotalCount = ko.observable(0);

    self.LoadResults = function () {
        var query = {
            AuthToken: cookie("authtoken"),
            StartAt: (self.PageNumber() - 1) * 20
        };

        self.self.PopulateQuery(query);

        self.IncrTasks();
        $.ajax({
            url: self.self.serviceAddress,
            data: ko.toJSON(query),
            success: function (response) {
                self.DecrTasks();
                self.Results(response.Results);
                self.TotalCount(response.TotalCount);
            }
        });
    };

    self.PrevPageCommand = function () {
        if (this.PageNumber() > 1) {
            this.PageNumber(this.PageNumber() - 1);
            this.LoadResults();
        }
    };

    self.NextPageCommand = function () {
        if (this.PageNumber() * 20 < self.TotalCount()) {
            this.PageNumber(this.PageNumber() + 1);
            this.LoadResults();
        }
    };
}