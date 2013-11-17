DeleteInterview.prototype = new ViewModel();
DeleteInterview.prototype.constructor = DeleteInterview;

function DeleteInterview(antiForgeryToken, interviewId) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;
    self.interviewId = interviewId;

    self.Name = ko.observable(null);

    self.DeleteCommand = function () {
        self.ErrorMessage(null);

        self.IncrTasks();
        $.ajax({
            url: "/services/interviews/DeleteInterview",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                AntiForgeryToken: self.antiForgeryToken,
                InterviewID: self.interviewId
            }),
            success: function (response) {
                self.DecrTasks();
                window.location = "/Interviews";
            }
        });
    };

    self.LoadInterview = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/interviews/GetInterview",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                InterviewID: self.interviewId
            }),
            success: function (response) {
                self.DecrTasks();
            }
        });
    };

    self.LoadInterview();
}