QuestionSearch.prototype = new ViewModel();
QuestionSearch.prototype.constructor = QuestionSearch;

function QuestionSearch() {
    var self = this;
    self.ViewModel(this);

    self.MaxQuestionsPerInterview = ko.observable(null);

    self.SearchResults = ko.observableArray([]);
    self.SelectedQuestions = ko.observableArray([]);
    self.QuestionNameFilter = ko.observable(null);

    self.LoadQuestions = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/questions/GetQuestions",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                Name: self.QuestionNameFilter(),
                StartAt: 0
            }),
            success: function (response) {
                self.DecrTasks();
                self.SearchResults(response.Results);
            }
        });
    };

    self.ShowAllQuestions = function () {
        self.QuestionNameFilter(null);
        self.LoadQuestions();
    };

    self.QuestionIsNotSelected = function ($root, $data) {
        return !_.any($root.SelectedQuestions(), function (q) { return q.ID === $data.ID; });
    };

    self.CanAddQuestion = function ($root, $data) {
        return self.QuestionIsNotSelected($root, $data) && (!$root.MaxQuestionsPerInterview() || $root.SelectedQuestions().length < $root.MaxQuestionsPerInterview());
    };

    self.AddQuestionCommand = function () {
        if (!_.include(self.SelectedQuestions.select(function (q) { return q.ID; })(), this.ID) &&
            (!self.MaxQuestionsPerInterview() || self.SelectedQuestions().length < self.MaxQuestionsPerInterview())) {
            self.SelectedQuestions.push(this);
        }
    };

    self.RemoveQuestionCommand = function () {
        self.SelectedQuestions.remove(this);
    };

    ko.computed(self.QuestionNameFilter)
        .extend({ throttle: 500 })
        .subscribe(function () {
            if (self.QuestionNameFilter()) {
                self.LoadQuestions();
            }
        });

    self.LoadQuestions();
}
