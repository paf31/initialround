CreateUser.prototype = new ViewModel();
CreateUser.prototype.constructor = CreateUser;

function CreateUser(antiForgeryToken) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;

    self.FirstName = ko.observable(null);
    self.LastName = ko.observable(null);
    self.Username = ko.observable(null);
    self.EmailAddress1 = ko.observable(null);
    self.EmailAddress2 = ko.observable(null);
    self.Password1 = ko.observable(null);
    self.Password2 = ko.observable(null);

    self.Validate = function () {
        if (!self.FirstName() || self.FirstName() == '' ||
            !self.LastName() || self.LastName() == '' ||
            !self.Username() || self.Username() == '' ||
            !self.EmailAddress1() || self.EmailAddress1() == '' ||
            !self.EmailAddress2() || self.EmailAddress2() == '' ||
            !self.Password1() || self.Password1() == '' ||
            !self.Password2() || self.Password2() == '') {
            self.ErrorMessage("Please fill all fields.");
            return false;
        }

        if (self.EmailAddress1() != self.EmailAddress2()) {
            self.ErrorMessage("Email addresses do not match.");
            return false;
        }

        if (!self.Password1() || self.Password2() == '') {
            self.ErrorMessage("Passwords do not match.");
            return false;
        }

        return true;
    };

    self.CreateCommand = function () {
        self.ErrorMessage(null);

        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/users/CreateUser",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    FirstName: self.FirstName(),
                    LastName: self.LastName(),
                    Username: self.Username(),
                    EmailAddress: self.EmailAddress1(),
                    Password: self.Password1()
                }),
                success: function (response) {
                    self.DecrTasks();
                    window.location = "/Users/Details/" + self.Username();
                }
            });
        }
    };
}