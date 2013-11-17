DeleteApplicant.prototype = new ViewModel();
DeleteApplicant.prototype.constructor = DeleteApplicant;

function DeleteApplicant(antiForgeryToken, applicantId) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;
    self.applicantId = applicantId;

    self.FirstName = ko.observable(null);
    self.LastName = ko.observable(null);

    self.DeleteCommand = function () {
        self.IncrTasks();
        self.ErrorMessage(null);

        $.ajax({
            url: "/services/applicants/DeleteApplicant",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                AntiForgeryToken: self.antiForgeryToken,
                ApplicantID: self.applicantId
            }),
            success: function (response) {
                self.DecrTasks();
                window.location = "/Applicants";
            }
        });
    };

    self.LoadApplicant = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/applicants/GetApplicant",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                AntiForgeryToken: self.antiForgeryToken,
                ApplicantID: self.applicantId
            }),
            success: function (response) {
                self.DecrTasks();
                self.FirstName(response.FirstName);
                self.LastName(response.LastName);
            }
        });
    }

    self.LoadApplicant();
}