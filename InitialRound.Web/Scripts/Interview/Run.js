Interview.prototype = new ViewModel();
Interview.prototype.constructor = Interview;

function Question(question) {
    var self = this;

    self.QuestionID = question.ID;
    self.Name = question.Name;
    self.QuestionBody = question.QuestionBody;
    self.AttemptToken = ko.observable(null);
    self.Submitted = ko.observable(question.Submitted);

    self.CanRunTests = ko.computed(function () {
        return self.AttemptToken() != null;
    });
}

function Interview(token) {
    var self = this;
    self.ViewModel(this);

    self.token = token;

    self.Step = ko.observable(0);

    self.MinutesAllowed = ko.observable(null);
    self.NumberOfQuestions = ko.observable(null);
    self.Questions = ko.observableArray([]);

    self.SelectedQuestion = ko.observable(null);

    self.Results = ko.observableArray([]);

    self.TimeRemaining = ko.observable(null);

    self.LoadInterview = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/interviews/LoadInterview",
            contentType: "application/json",
            data: ko.toJSON({
                Token: self.token
            }),
            success: function (response) {
                self.DecrTasks();

                self.MinutesAllowed(response.MinutesAllowed);
                self.NumberOfQuestions(response.NumberOfQuestions);

                switch (response.StatusID) {
                    case 1:
                        self.Step(1);
                        break;
                    case 2:
                        self.BeginInterviewCommand();
                        break;
                    case 3:
                    case 4:
                        self.Step(3);
                        break;
                    default:
                        self.ErrorMessage("Cannot open this page at this time.");
                        break;
                }
            }
        });
    };

    self.BeginInterviewCommand = function () {
        self.Step(2);

        self.IncrTasks();
        $.ajax({
            url: "/services/interviews/StartInterview",
            data: ko.toJSON({
                Token: self.token
            }),
            success: function (response) {
                self.DecrTasks();

                for (var i = 0; i < response.Questions.length; i++) {
                    self.Questions.push(new Question(response.Questions[i]));
                }

                self.SelectQuestionCommand(self.Questions()[0]);
                self.initialSecondsRemaining = response.SecondsRemaining;
                self.startTime = new Date();
                self.StartTimer();
            }
        });

        window.onbeforeunload = function () {
            if (self.Step() == 2) {
                return "Are you sure you would like to exit? Please make sure you have submitted all answers.";
            }
        }
    };

    self.Validate = function () {
        self.ErrorMessage(null);

        var codeFile = $('#pvCode').val();
        var outputFile = $('#pvOutput').val();

        if (outputFile == null || outputFile == '') {
            self.ErrorMessage("Please select an output file to upload.");
            return false;
        }

        if (codeFile == null || codeFile == '') {
            self.ErrorMessage("Please select a code file to upload.");
            return false;
        }

        return true;
    }

    self.DownloadInputCommand = function () {
        var selectedQuestion = self.SelectedQuestion();

        var uri = '/Services/DownloadInput.ashx?Token={0}&QuestionID={1}'
            .replace('{0}', encodeURIComponent(self.token))
            .replace('{1}', encodeURIComponent(selectedQuestion.QuestionID));

        self.IncrTasks();

        $.ajax({
            url: "/services/interviews/CreateAttemptToken",
            data: ko.toJSON({
                Token: self.token,
                QuestionID: selectedQuestion.QuestionID
            }),
            success: function (response) {
                self.DecrTasks();

                selectedQuestion.AttemptToken(response.AttemptToken);

                uri = uri + '&AttemptToken=' + encodeURIComponent(selectedQuestion.AttemptToken());

                window.open(uri, '_blank');

                $('#pvForm').get(0).reset();
            }
        });
    };

    self.RunTestsCommand = function () {
        if (self.Validate()) {
            self.IncrTasks();
            self.Results([]);
            var selectedQuestion = self.SelectedQuestion();

            var data = {
                Token: self.token,
                QuestionID: selectedQuestion.QuestionID,
                AttemptToken: selectedQuestion.AttemptToken()
            };


            $('#pvForm').ajaxSubmit({
                type: 'POST',
                url: '/Services/UploadOutput.ashx',
                dataType: 'json',
                data: data,
                success: function (response) {
                    self.DecrTasks();

                    selectedQuestion.AttemptToken(null);
                    selectedQuestion.Submitted(true);

                    if (response.ErrorMessage) {
                        self.ErrorMessage(response.ErrorMessage);
                    } else {
                        self.Results(response.TestResults);
                    }
                },
                error: function () {
                    self.DecrTasks();
                    self.ErrorMessage('An unknown error has occurred.');
                }
            });
        }
    };

    self.SelectQuestionCommand = function (question) {
        self.SelectedQuestion(question);
    };

    self.Tick = function () {
        var secondsElapsed = (new Date().getTime() - self.startTime.getTime()) / 1000;
        var totalSecondsRemaining = Math.max(0, self.initialSecondsRemaining - secondsElapsed);
        var hoursRemaining = Math.floor(totalSecondsRemaining / 3600);
        var minutesRemaining = Math.floor((totalSecondsRemaining / 60) % 60);
        var secondsRemaining = Math.floor(totalSecondsRemaining % 60);
        var hoursRemainingString = hoursRemaining < 10 ? '0' + hoursRemaining : '' + hoursRemaining;
        var minutesRemainingString = minutesRemaining < 10 ? '0' + minutesRemaining : '' + minutesRemaining;
        var secondsRemainingString = secondsRemaining < 10 ? '0' + secondsRemaining : '' + secondsRemaining;
        var timeRemainingString = hoursRemaining > 0
            ? hoursRemainingString + ':' + minutesRemainingString + ':' + secondsRemainingString
            : minutesRemainingString + ':' + secondsRemainingString;
        self.TimeRemaining(timeRemainingString);

        if (totalSecondsRemaining == 0) {
            self.StopTimer();
            self.Step(3);
        }
    };

    self.StartTimer = function () {
        self.timerId = window.setInterval(function () {
            self.Tick();
        }, 1000);
    };

    self.StopTimer = function () {
        if (self.timerId) {
            window.clearInterval(self.timerId);
            self.timerId = undefined;
        }
    };

    self.LoadInterview();
}