Account.prototype = new ViewModel();
Account.prototype.constructor = Account;

function Account() {
    var self = this;
    self.ViewModel(this);

    self.IsFreeAccount = ko.observable(null);

    self.LoadPlan = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/account/IsFreeAccount",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
            }),
            success: function (response) {
                self.DecrTasks();
                self.IsFreeAccount(response.IsFreeAccount);
            }
        });
    };

    self.LoadPlan();
}