Print.prototype = new ViewModel();
Print.prototype.constructor = Print;

function Print(interviewId) {
    var self = this;
    self.ViewModel(this);

    self.interviewId = interviewId;

    self.Response = ko.observable(null);

    self.LoadSummary = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/interviews/GetInterviewSummary",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                InterviewID: self.interviewId
            }),
            success: function (response) {
                self.DecrTasks();
                self.Response(response);
            }
        });
    };

    self.LoadSummary();
}
