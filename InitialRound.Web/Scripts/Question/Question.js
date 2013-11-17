Question.prototype = new ViewModel();
Question.prototype.constructor = Question;

function Test(id) {
    var self = this;

    self.ID = id;
    self.Name = ko.observable(null);
    self.Input = ko.observable(null);
    self.ExpectedOutput = ko.observable(null);
}

function Question(antiForgeryToken, questionId) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;

    self.QuestionID = ko.observable(questionId);

    self.SelectedTab = ko.observable(0);

    self.Name = ko.observable(null);
    self.CanEdit = ko.observable(true);
    self.IsCodedTest = ko.observable(false);
    self.QuestionBody = ko.observable(null);

    self.PreviewResults = ko.observableArray([]);

    self.EditingMarkdown = ko.observable(false);

    self.Tests = ko.observableArray([]);

    self.Validate = function () {
        if (!self.Name() || self.Name() == '') {
            self.ErrorMessage("Please fill in the question name.");
            return false;
        }

        if (_.any(self.Tests(), function (t) { return !t.Name() || t.Name() == ''; })) {
            self.ErrorMessage("Please fill in the name for each test.");
            return false;
        }

        return true;
    };

    self.CreateCommand = function () {
        self.ErrorMessage(null);

        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/questions/CreateQuestion",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    Name: self.Name(),
                    QuestionBody: self.QuestionBody(),
                    Tests: self.Tests()
                }),
                success: function (response) {
                    self.DecrTasks();
                    self.HasChanges(false);

                    window.location = "/Questions/Details/" + response.QuestionID;
                }
            });
        }
    };

    self.LoadQuestion = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/questions/GetQuestion",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                QuestionID: self.QuestionID()
            }),
            success: function (response) {
                self.DecrTasks();
                self.Name(response.Name);
                self.CanEdit(response.CanEdit);
                self.IsCodedTest(response.IsCodedTest);
                self.QuestionBody(response.QuestionBody);

                self.Tests(_.map(response.Tests, function (test) {
                    var newTest = new Test(test.ID);

                    newTest.Name(test.Name);
                    newTest.Input(test.Input);
                    newTest.ExpectedOutput(test.ExpectedOutput);

                    return newTest;
                }));

                self.AddEventHandlers();
                self.HasChanges(false);
            }
        });
    };

    self.CopyCommand = function () {
        self.ErrorMessage(null);

        if (self.HasChanges()) {
            self.ErrorMessage("There are unsaved changes.");
        }
        else {
            self.IncrTasks();
            $.ajax({
                url: "/services/questions/CreateQuestion",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    Name: "Copy of " + self.Name(),
                    QuestionBody: self.QuestionBody(),
                    Tests: _.map(self.Tests(), function (test) {
                        return {
                            ID: test.ID,
                            Name: test.Name(),
                            Input: test.Input(),
                            ExpectedOutput: test.ExpectedOutput()
                        }
                    })
                }),
                success: function (response) {
                    self.DecrTasks();
                    window.location = "/Questions/Details/" + response.QuestionID;
                }
            });
        }
    }

    self.SaveCommand = function () {
        self.ErrorMessage(null);

        if (self.Validate() && self.CanEdit()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/questions/EditQuestion",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    QuestionID: self.QuestionID(),
                    Name: self.Name(),
                    QuestionBody: self.QuestionBody(),
                    Tests: self.Tests()
                }),
                success: function (response) {
                    self.DecrTasks();

                    if (response.TestIDs) {
                        for (var i = 0; i < response.TestIDs.length; i++) {
                            self.Tests()[i].ID = response.TestIDs[i];
                        }
                    }

                    self.HasChanges(false);
                }
            });
        }
    };

    self.AddTestCommand = function () {
        var test = new Test(null);
        test.Name("New Test");
        self.Tests.push(test);
        self.HasChanges(true);
    };

    self.RemoveTestCommand = function () {
        self.Tests.remove(this);
        self.HasChanges(true);
    };

    self.DownloadInputCommand = function () {
        self.ErrorMessage(null);

        if (self.CheckQuestionIsSaved()) {
            var uri = '/Services/DownloadInput.ashx?AuthToken={0}&QuestionID={1}'
                .replace('{0}', encodeURIComponent(cookie('authtoken')))
                .replace('{1}', encodeURIComponent(self.QuestionID()));

            window.open(uri, '_blank');

            $('#pvForm').get(0).reset();
        }
    };

    self.RunTests = function () {
        self.IncrTasks();

        self.PreviewResults([]);

        $('#pvForm').ajaxSubmit({
            type: 'POST',
            url: '/Services/UploadOutput.ashx',
            dataType: 'json',
            data: {
                AuthToken: cookie("authtoken"),
                AntiForgeryToken: self.antiForgeryToken,
                QuestionID: self.QuestionID()
            },
            success: function (response) {
                self.DecrTasks();

                if (response.ErrorMessage) {
                    self.ErrorMessage(response.ErrorMessage);
                } else {
                    self.PreviewResults(response.Tests);
                }
            },
            error: function () {
                self.DecrTasks();
                self.ErrorMessage('An unknown error has occurred.');
            }
        });
    };

    self.RunTestsCommand = function () {
        self.ErrorMessage(null);

        if (self.ValidatePreview()) {
            self.RunTests();
        }
    };

    self.CheckQuestionIsSaved = function () {
        if (self.QuestionID() == null) {
            self.ErrorMessage("Question is not saved.");
            return false;
        }

        if (self.HasChanges()) {
            self.ErrorMessage("Please save changes before running tests.");
            return false;
        }

        return true;
    }

    self.ValidatePreview = function () {
        var outputFile = $('#pvOutput').val();

        if (outputFile == null || outputFile == '') {
            self.ErrorMessage("Please select a file to upload.");
            return false;
        }

        return self.CheckQuestionIsSaved();
    };

    self.UploadCSVTestsCommand = function () {
        var outputFile = $('#csvFile').val();

        if (outputFile == null || outputFile == '') {
            self.ErrorMessage("Please select a file to upload.");
            return false;
        }

        self.IncrTasks();

        self.PreviewResults([]);

        $('#csvForm').ajaxSubmit({
            type: 'POST',
            url: '/Services/UploadTestsCSV.ashx',
            dataType: 'json',
            data: {
                AuthToken: cookie("authtoken"),
                AntiForgeryToken: self.antiForgeryToken,
                QuestionID: self.QuestionID()
            },
            success: function (response) {
                self.DecrTasks();

                if (response.ErrorMessage) {
                    self.ErrorMessage(response.ErrorMessage);
                } else {
                    self.Tests(_.map(response.Tests, function (test) {
                        var newTest = new Test(test.ID);

                        newTest.Name(test.Name);
                        newTest.Input(test.Input);
                        newTest.ExpectedOutput(test.ExpectedOutput);

                        return newTest;
                    }));
                }
            },
            error: function (response) {
                self.DecrTasks();
                self.ErrorMessage('An unknown error has occurred.');
            }
        });
    }

    self.AddEventHandlers = function () {
        self.Name.subscribe(function () { self.HasChanges(true); });
        self.QuestionBody.subscribe(function () { self.HasChanges(true); });
        self.Tests.subscribe(function () { self.HasChanges(true); });

        ko.computed(function () {
            return _.map(self.Tests(), function (t) { return t.Name(); });
        }).subscribe(function () { self.HasChanges(true); });
    };

    if (self.QuestionID() != null) {
        self.LoadQuestion();
    } else {
        self.AddEventHandlers();
    }
}
