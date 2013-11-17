function Attempt(attempt) {
    var self = this;

    self.AttemptID = attempt.AttemptID;
    self.TimeOffset = attempt.TimeOffset;

    self.Loaded = false;
    self.Code = ko.observable(null);
    self.Output = ko.observable(null);
    self.Results = ko.observableArray([]);
}

function InterviewQuestion(question) {
    var self = this;

    self.Question = question;
    self.Loaded = false;

    self.Attempts = ko.observableArray([]);
}

Interview.prototype = new ViewModel();
Interview.prototype.constructor = Interview;

function Interview(antiForgeryToken, interviewId) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;

    self.interviewId = interviewId;

    self.ApplicantID = ko.observable(null);
    self.ApplicantName = ko.observable(null);
    self.Status = ko.observable(null);
    self.StatusText = ko.observable(null);
    self.TimeRemaining = ko.observable(null);
    self.MinutesAllowed = ko.observable(null);

    self.QuestionSearch = new QuestionSearch();
    self.QuestionSearch.Busy.subscribe(self.Busy);

    self.Questions = ko.observableArray([]);

    self.SelectedQuestion = ko.observable(null);
    self.SelectedAttempt = ko.observable(null);

    self.Validate = function () {
        if (!self.MinutesAllowed() || isNaN(self.MinutesAllowed())) {
            self.ErrorMessage("Please enter the minutes allowed.");
            return false;
        }

        return true;
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
                self.ApplicantID(response.ApplicantID);
                self.ApplicantName(response.ApplicantName);
                self.Status(response.Status);
                self.StatusText(response.StatusText);
                self.TimeRemaining(response.TimeRemaining);
                self.MinutesAllowed(response.MinutesAllowed);
                self.QuestionSearch.SelectedQuestions(response.Questions);
                self.Questions(_.map(response.Questions, function (question) {
                    return new InterviewQuestion(question);
                }));
                self.SelectQuestionCommand(_.first(self.Questions()));
                self.HasChanges(false);
            }
        });
    };

    self.SelectQuestionCommand = function (question) {
        self.SelectedQuestion(null);

        if (question.Loaded) {
            self.SelectedQuestion(question);
            self.LoadAttemptCommand(_.first(question.Attempts()));
        } else {
            self.IncrTasks();
            $.ajax({
                url: "/services/interviews/GetQuestionAttempts",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    QuestionID: question.Question.ID,
                    InterviewQuestionID: question.Question.InterviewQuestionID,
                    InterviewID: self.interviewId
                }),
                success: function (response) {
                    self.DecrTasks();
                    question.Loaded = true;
                    question.Attempts(_.map(response.Attempts, function (attempt) {
                        return new Attempt(attempt);
                    }));
                    self.SelectedQuestion(question);
                    self.LoadAttemptCommand(_.first(question.Attempts()));
                }
            });
        }
    };

    self.LoadAttemptCommand = function (attempt) {
        if (attempt != null) {
            if (attempt.Loaded) {
                self.SelectedAttempt(attempt);
            } else {
                self.IncrTasks();
                $.ajax({
                    url: "/services/interviews/GetAttemptDetails",
                    data: ko.toJSON({
                        AuthToken: cookie("authtoken"),
                        AttemptID: attempt.AttemptID,
                    }),
                    success: function (response) {
                        self.DecrTasks();
                        attempt.Loaded = true;
                        attempt.Code(response.Code);
                        attempt.Output(response.Output);
                        attempt.Results(response.Results);
                        self.SelectedAttempt(attempt);
                    }
                });
            }
        } else {
            self.SelectedAttempt(null);
        }
    }

    self.SendInvitationCommand = function () {
        self.ErrorMessage(null);

        if (self.HasChanges()) {
            self.ErrorMessage("There are unsaved changes.");
        } else if (self.Status() <= 1) {
            self.IncrTasks();
            $.ajax({
                url: "/services/interviews/SendInvitation",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    InterviewID: self.interviewId
                }),
                success: function (response) {
                    self.DecrTasks();
                    self.Status(1);
                    self.StatusText("Invitation Sent");
                }
            });
        }
    };

    self.SaveCommand = function () {
        self.ErrorMessage(null);

        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/interviews/EditInterview",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    InterviewID: self.interviewId,
                    Status: self.Status(),
                    MinutesAllowed: self.MinutesAllowed(),
                    QuestionIDs: self.QuestionSearch.SelectedQuestions.select(function (q) { return q.ID })()
                }),
                success: function (response) {
                    self.DecrTasks();
                    self.HasChanges(false);
                }
            });
        }
    };

    self.LoadInterview();

    self.Status.subscribe(function () { self.HasChanges(true); });
    self.MinutesAllowed.subscribe(function () { self.HasChanges(true); });
    self.QuestionSearch.HasChanges.subscribe(function (value) {
        if (value == true) {
            self.HasChanges(true);
        }
    });
}
