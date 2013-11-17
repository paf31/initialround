CreateInterview.prototype = new ViewModel();
CreateInterview.prototype.constructor = CreateInterview;

function CreateInterview(antiForgeryToken, applicantId) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;
    self.applicantId = applicantId;

    self.Type = ko.observable(null);

    self.MinutesAllowed = ko.observable(10);

    self.QuestionSearch = new QuestionSearch();
    self.QuestionSearch.Busy.subscribe(self.Busy);
    self.QuestionSearch.HasChanges.subscribe(function (value) {
        if (value == true) {
            self.HasChanges(true);
        }
    });

    self.QuestionSets = ko.observableArray([]);
    self.QuestionSetID = ko.observable(null);

    self.LoadQuestionSets = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/questionsets/GetQuestionSets",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                StartAt: 0
            }),
            success: function (response) {
                self.DecrTasks();
                self.QuestionSets(response.Results);
            }
        });
    };

    self.Validate = function () {
        if (!self.MinutesAllowed() || isNaN(self.MinutesAllowed())) {
            self.ErrorMessage("Please enter the minutes allowed.");
            return false;
        }

        if (!self.Type()) {
            self.ErrorMessage("Please select a question set or at least one question.");
            return false;
        }

        if (self.Type() === 'QuestionSet' && self.QuestionSetID() == null) {
            self.ErrorMessage("Please select a question set.");
            return false;
        }

        if (self.Type() === 'Questions' && !_.any(self.QuestionSearch.SelectedQuestions())) {
            self.ErrorMessage("Please select at least one question.");
            return false;
        }

        return true;
    };

    self.CreateCommand = function () {
        self.ErrorMessage(null);

        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/interviews/CreateInterview",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    ApplicantID: self.applicantId,
                    MinutesAllowed: self.MinutesAllowed(),
                    UseQuestionSet: self.Type() === 'QuestionSet',
                    QuestionIDs: self.Type() === 'QuestionSet' ? [] : self.QuestionSearch.SelectedQuestions.select(function (q) { return q.ID })(),
                    QuestionSetID: self.Type() === 'QuestionSet' ? self.QuestionSetID() : null
                }),
                success: function (response) {
                    self.DecrTasks();
                    self.HasChanges(false);
                    window.location = '/Interviews/Details/' + response.InterviewID;
                }
            });
        }
    };

    self.MinutesAllowed.subscribe(function () { self.HasChanges(true); });
    self.QuestionSetID.subscribe(function () { self.HasChanges(true); });

    self.LoadQuestionSets();
}
