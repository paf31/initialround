Login.prototype = new ViewModel();
Login.prototype.constructor = Login;

function Login() {
    var self = this;
    self.ViewModel(this);

    self.Username = ko.observable(null);
    self.Password = ko.observable(null);

    self.Validate = function () {
        self.ErrorMessage(null);

        if (self.Username() == null || self.Username() == '') {
            self.ErrorMessage("Please enter your username.");
            return false;
        }

        if (self.Password() == null || self.Password() == '') {
            self.ErrorMessage("Please enter your password.");
            return false;
        }

        return true;
    };

    self.LoginCommand = function () {
        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/account/Login",
                data: ko.toJSON({
                    Username: self.Username(),
                    Password: self.Password()
                }),
                success: function (response) {
                    self.DecrTasks();

                    var expiryDate = new Date();
                    expiryDate.setSeconds(expiryDate.getSeconds() + response.ExpirySeconds);

                    document.cookie = "authtoken=" + response.AuthToken + "; path=/; secure; expires=" + expiryDate.toUTCString();
                    document.cookie = "expires=" + escape(expiryDate.toUTCString()) + "; path=/; secure; expires=" + expiryDate.toUTCString();

                    window.location = '/';
                }
            });
        }
    };
}