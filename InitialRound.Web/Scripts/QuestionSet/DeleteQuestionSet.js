DeleteQuestionSet.prototype = new ViewModel();
DeleteQuestionSet.prototype.constructor = DeleteQuestionSet;

function DeleteQuestionSet(antiForgeryToken, questionSetId) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;
    self.questionSetId = questionSetId;

    self.Name = ko.observable(null);

    self.DeleteCommand = function () {
        self.ErrorMessage(null);

        self.IncrTasks();
        $.ajax({
            url: "/services/questionsets/DeleteQuestionSet",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                AntiForgeryToken: self.antiForgeryToken,
                QuestionSetID: self.questionSetId
            }),
            success: function (response) {
                self.DecrTasks();
                window.location = "/QuestionSets";
            }
        });
    };

    self.LoadQuestionSet = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/questionsets/GetQuestionSet",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                QuestionSetID: self.questionSetId
            }),
            success: function (response) {
                self.DecrTasks();
                self.Name(response.Name);
            }
        });
    };

    self.LoadQuestionSet();
}