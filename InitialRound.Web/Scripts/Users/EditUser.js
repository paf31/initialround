EditUser.prototype = new ViewModel();
EditUser.prototype.constructor = EditUser;

function EditUser(antiForgeryToken, username) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;
    self.username = username;

    self.FirstName = ko.observable(null);
    self.LastName = ko.observable(null);
    self.EmailAddress = ko.observable(null);

    self.Validate = function () {
        if (!self.FirstName() || self.FirstName() == '' ||
            !self.LastName() || self.LastName() == '' ||
            !self.EmailAddress() || self.EmailAddress() == '') {
            self.ErrorMessage("Please fill in the first name, last name and email address fields.");
            return false;
        }

        return true;
    };

    self.SaveCommand = function () {
        self.ErrorMessage(null);

        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/users/EditUser",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    Username: self.username,
                    FirstName: self.FirstName(),
                    LastName: self.LastName(),
                    EmailAddress: self.EmailAddress()
                }),
                success: function (response) {
                    self.DecrTasks();
                }
            });
        }
    };

    self.LoadUser = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/users/GetUser",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                AntiForgeryToken: self.antiForgeryToken,
                Username: self.username
            }),
            success: function (response) {
                self.DecrTasks();
                self.FirstName(response.FirstName);
                self.LastName(response.LastName);
                self.EmailAddress(response.EmailAddress);
            }
        });
    }

    self.LoadUser();
}
