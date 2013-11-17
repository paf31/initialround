DeleteQuestion.prototype = new ViewModel();
DeleteQuestion.prototype.constructor = DeleteQuestion;

function DeleteQuestion(antiForgeryToken, questionId) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;
    self.questionId = questionId;

    self.Name = ko.observable(null);

    self.DeleteCommand = function () {
        self.ErrorMessage(null);

        self.IncrTasks();
        $.ajax({
            url: "/services/questions/DeleteQuestion",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                AntiForgeryToken: self.antiForgeryToken,
                QuestionID: self.questionId
            }),
            success: function (response) {
                self.DecrTasks();
                window.location = "/Questions";
            }
        });
    };

    self.LoadQuestion = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/questions/GetQuestion",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                QuestionID: self.questionId
            }),
            success: function (response) {
                self.DecrTasks();
                self.Name(response.Name);
            }
        });
    };

    self.LoadQuestion();
}