ResetPassword.prototype = new ViewModel();
ResetPassword.prototype.constructor = ResetPassword;

function ResetPassword() {
    var self = this;
    self.ViewModel(this);

    self.Username = ko.observable(null);

    self.Success = ko.observable(false);

    self.Validate = function () {
        self.ErrorMessage(null);

        if (self.Username() == null || self.Username() == '') {
            self.ErrorMessage("Please enter your username.");
            return false;
        }

        return true;
    };

    self.ResetPasswordCommand = function () {
        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/account/ResetPassword",
                data: ko.toJSON({
                    Username: self.Username(),
                }),
                success: function (response) {
                    self.DecrTasks();

                    self.Success(true);
                }
            });
        }
    };
}