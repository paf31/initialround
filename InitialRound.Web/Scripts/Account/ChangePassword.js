ChangePassword.prototype = new ViewModel();
ChangePassword.prototype.constructor = ChangePassword;

function ChangePassword(antiForgeryToken) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;

    self.OldPassword = ko.observable(null);
    self.NewPassword1 = ko.observable(null);
    self.NewPassword2 = ko.observable(null);

    self.Success = ko.observable(false);

    self.Validate = function () {
        self.ErrorMessage(null);

        if (self.OldPassword() == null || self.OldPassword() == '') {
            self.ErrorMessage("Please enter your old password.");
            return false;
        }

        if (self.NewPassword1() == null || self.NewPassword1() == '' || self.NewPassword2() == null || self.NewPassword2() == '') {
            self.ErrorMessage("Please enter a new password.");
            return false;
        }

        if (self.NewPassword1() != self.NewPassword2()) {
            self.ErrorMessage("Passwords do not match.");
            return false;
        }

        if (self.NewPassword1().length < 6 ||
            !(new RegExp("[a-z]").test(self.NewPassword1())) ||
            !(new RegExp("[A-Z]").test(self.NewPassword1())) ||
            !(new RegExp("[0-9]").test(self.NewPassword1()))) {
            self.ErrorMessage("Your password must be at least 6 characters long, and contain at least one upper case character, one lower case character and one numeric digit.");
            return false;
        }

        return true;
    };

    self.ChangePasswordCommand = function () {
        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/account/ChangePassword",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    OldPassword: self.OldPassword(),
                    NewPassword: self.NewPassword1()
                }),
                success: function (response) {
                    self.DecrTasks();
                    self.Success(true);
                }
            });
        }
    }
}
