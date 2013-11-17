BulkImport.prototype = new ViewModel();
BulkImport.prototype.constructor = BulkImport;

function BulkImport(antiForgeryToken) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;

    self.Applicants = ko.observableArray([]);

    self.UploadCSVApplicantsCommand = function () {
        var outputFile = $('#csvFile').val();

        if (outputFile == null || outputFile == '') {
            self.ErrorMessage("Please select a file to upload.");
            return false;
        }

        self.IncrTasks();

        self.Applicants([]);

        $('#csvForm').ajaxSubmit({
            type: 'POST',
            url: '/Services/UploadApplicantsCSV.ashx',
            dataType: 'json',
            data: {
                AuthToken: cookie("authtoken"),
                AntiForgeryToken: self.antiForgeryToken
            },
            success: function (response) {
                self.DecrTasks();

                if (response.ErrorMessage) {
                    self.ErrorMessage(response.ErrorMessage);
                } else {
                    self.Applicants(response.Applicants);
                }
            },
            error: function (response) {
                self.DecrTasks();
                self.ErrorMessage('An unknown error has occurred.');
            }
        });
    }

    self.CreateApplicantsCommand = function () {
        if (_.any(self.Applicants())) {
            self.IncrTasks();
            $.ajax({
                url: "/services/applicants/BulkCreateApplicants",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    Applicants: self.Applicants()
                }),
                success: function (response) {
                    self.DecrTasks();
                    window.location = '/Applicants';
                }
            });
        }
    };
}
