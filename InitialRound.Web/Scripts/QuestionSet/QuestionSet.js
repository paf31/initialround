QuestionSet.prototype = new ViewModel();
QuestionSet.prototype.constructor = QuestionSet;

function QuestionSet(antiForgeryToken, questionSetId) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;

    self.QuestionSetID = ko.observable(questionSetId);

    self.Name = ko.observable(null);

    self.QuestionSearch = new QuestionSearch();
    self.QuestionSearch.Busy.subscribe(self.Busy);
    self.QuestionSearch.HasChanges.subscribe(function (value) {
        if (value == true) {
            self.HasChanges(true);
        }
    });

    self.Validate = function () {
        if (!self.Name() || self.Name() == '') {
            self.ErrorMessage("Please fill in the question set name.");
            return false;
        }

        if (self.QuestionSearch.SelectedQuestions().length == 0) {
            self.ErrorMessage("Please select at least one question.");
            return false;
        }

        return true;
    };

    self.LoadQuestionSet = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/questionsets/GetQuestionSet",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                QuestionSetID: self.QuestionSetID()
            }),
            success: function (response) {
                self.DecrTasks();
                self.Name(response.Name);
                self.QuestionSearch.SelectedQuestions(response.Questions);
                self.AddEventHandlers();
            }
        });
    };

    self.SaveCommand = function () {
        self.ErrorMessage(null);

        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/questionsets/EditQuestionSet",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    QuestionSetID: self.QuestionSetID(),
                    Name: self.Name(),
                    QuestionIDs: _.map(self.QuestionSearch.SelectedQuestions(), function (q) { return q.ID; })
                }),
                success: function (response) {
                    self.DecrTasks();
                }
            });
        }
    };

    self.CreateCommand = function () {
        self.ErrorMessage(null);

        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/questionsets/CreateQuestionSet",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    Name: self.Name(),
                    QuestionIDs: self.QuestionSearch.SelectedQuestions.select(function (q) { return q.ID })()
                }),
                success: function (response) {
                    self.DecrTasks();
                    self.HasChanges(false);

                    window.location = "/QuestionSets/Details/" + response.QuestionSetID;
                }
            });
        }
    };

    self.AddEventHandlers = function () {
        self.Name.subscribe(function () { self.HasChanges(true); });
        self.QuestionSearch.SelectedQuestions.subscribe(function () { self.HasChanges(true); });
    };

    if (self.QuestionSetID() != null) {
        self.LoadQuestionSet();
    } else {
        self.AddEventHandlers();
    }
}
