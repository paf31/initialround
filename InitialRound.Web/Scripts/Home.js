Home.prototype = new ViewModel();
Home.prototype.constructor = Home;

function Home(antiForgeryToken) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;

    self.NewInterviews = ko.observable(0);
    self.PendingInterviews = ko.observable(0);
    self.InterviewsInProgress = ko.observable(0);
    self.InterviewsRequiringReview = ko.observable(0);

    self.EmailAddress = ko.observable(null);
    self.MinutesAllowed = ko.observable(10);
    self.SendInvitation = ko.observable(false);
    self.QuestionSetID = ko.observable(null);
    self.QuestionSets = ko.observableArray(null);

    self.Validate = function () {
        if (!self.EmailAddress() || self.EmailAddress() == '') {
            self.ErrorMessage("Please fill in the email address field.");
            return false;
        }

        var emailAddress = /\S+@\S+\.\S+/;

        if (self.EmailAddress() && !emailAddress.test(self.EmailAddress())) {
            self.ErrorMessage("Please enter a valid email address.");
            return false;
        } 

        if (!self.MinutesAllowed() || isNaN(self.MinutesAllowed()) || self.MinutesAllowed() < 1) {
            self.ErrorMessage("Please enter the number of minutes in the duration field.");
            return false;
        }

        if (self.QuestionSetID() == null) {
            self.ErrorMessage("Please select a question set.");
            return false;
        }

        return true;
    };

    self.QuickStartCommand = function () {
        self.ErrorMessage(null);
        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/interviews/QuickStart",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    EmailAddress: self.EmailAddress(),
                    MinutesAllowed: self.MinutesAllowed(),
                    SendInvitation: self.SendInvitation(),
                    QuestionSetID: self.QuestionSetID()
                }),
                success: function (response) {
                    self.DecrTasks();
                    window.location = '/Interviews/Details/' + response.InterviewID;
                }
            });
        }
    };

    self.IncrTasks();
    $.ajax({
        url: "/services/reports/HomePageReport",
        data: ko.toJSON({
            AuthToken: cookie("authtoken")
        }),
        success: function (response) {
            self.DecrTasks();
            self.NewInterviews(response.NewInterviews);
            self.PendingInterviews(response.PendingInterviews);
            self.InterviewsInProgress(response.InterviewsInProgress);
            self.InterviewsRequiringReview(response.InterviewsRequiringReview);
        }
    });
    
    self.IncrTasks();
    $.ajax({
        url: "/services/questionsets/GetQuestionSets",
        data: ko.toJSON({
            AuthToken: cookie("authtoken")
        }),
        success: function (response) {
            self.DecrTasks();
            self.QuestionSets(response.Results);
        }
    });
}