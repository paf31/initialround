Practice.prototype = new ViewModel();
Practice.prototype.constructor = Practice;

function Practice() {
    var self = this;
    self.ViewModel(this);

    self.ShowExamples = ko.observable(false);

    self.Submitted = ko.observable(false);
    self.Results = ko.observableArray([]);

    self.Validate = function () {
        self.ErrorMessage(null);

        var codeFile = $('#pvCode').val();
        var outputFile = $('#pvOutput').val();

        if (outputFile == null || outputFile == '') {
            self.ErrorMessage("Please select an output file to upload.");
            return false;
        }

        if (codeFile == null || codeFile == '') {
            self.ErrorMessage("Please select a code file to upload.");
            return false;
        }

        return true;
    }

    self.DownloadInputCommand = function () {
        window.open('/Services/DownloadInput.ashx?Practice=1', '_blank');

        $('#pvForm').get(0).reset();
    };

    self.ShowExamplesCommand = function () {
        self.ShowExamples(true);
    };

    self.RunTestsCommand = function () {
        if (self.Validate()) {
            self.IncrTasks();
            self.Results([]);

            $('#pvForm').ajaxSubmit({
                type: 'POST',
                url: '/Services/UploadOutput.ashx',
                dataType: 'json',
                data: { Practice: true },
                success: function (response) {
                    self.DecrTasks();

                    self.Submitted(true);

                    if (response.ErrorMessage) {
                        self.ErrorMessage(response.ErrorMessage);
                    } else {
                        self.Results(response.TestResults);
                    }
                },
                error: function () {
                    self.DecrTasks();
                    self.ErrorMessage('An unknown error has occurred.');
                }
            });
        }
    };
}