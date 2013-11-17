DeleteUser.prototype = new ViewModel();
DeleteUser.prototype.constructor = DeleteUser;

function DeleteUser(antiForgeryToken, username) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;
    self.username = username;


    self.DeleteCommand = function () {
        self.ErrorMessage(null);

        self.IncrTasks();
        $.ajax({
            url: "/services/users/DeleteUser",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                AntiForgeryToken: self.antiForgeryToken,
                Username: self.username
            }),
            success: function (response) {
                self.DecrTasks();
                window.location = "/Users";
            }
        });
    };
}