function ViewModel() {
    var self = this;

    self.self = self;

    self.HasChanges = ko.observable(false);

    self.ViewModel = function () {
        self.self = this;
    };

    self.TaskCount = ko.observable(0);
    self.Busy = ko.computed({
        read: function () {
            return self.TaskCount() > 0;
        },
        write: function (value) {
        }
    }).extend({ throttle: 500 });

    self.IncrTasks = function () {
        self.TaskCount(self.TaskCount() + 1);
    };

    self.DecrTasks = function () {
        self.TaskCount(self.TaskCount() - 1);
    };

    self.ErrorMessage = ko.observable(null);

    $.ajaxSetup({
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        headers: { "cache-control": "no-cache" },
        success: function (response) {
            self.DecrTasks();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            self.DecrTasks();
            var errorMessage = $.parseJSON(jqXHR.responseText);
            self.ErrorMessage(errorMessage);
        }
    });

    self.setKeepAliveTimeout = function () {
        if (window.cookie) {
            var expires = cookie("expires");

            if (expires) {
                var expiresDate = new Date(expires);
                var fiveMinutesBeforeExpiry = expiresDate.getTime() - 300000;
                var ticks = fiveMinutesBeforeExpiry - new Date().getTime();

                if (ticks > 0) {
                    window.setTimeout(function () {
                        self.KeepSessionAlive();
                    }, ticks);
                } else {
                    self.KeepSessionAlive();
                }
            }
        }
    };

    self.KeepSessionAlive = function () {
        var authToken = cookie("authtoken");

        if (authToken) {
            $.ajax({
                url: "/services/account/KeepAlive",
                data: ko.toJSON({
                    AuthToken: authToken,
                }),
                success: function (response) {
                    var expiryDate = new Date();
                    expiryDate.setSeconds(expiryDate.getSeconds() + response.ExpirySeconds);

                    document.cookie = "authtoken=" + response.NewAuthToken + "; path=/; secure; expires=" + expiryDate.toUTCString();
                    document.cookie = "expires=" + escape(expiryDate.toUTCString()) + "; path=/; secure; expires=" + expiryDate.toUTCString();
                    self.setKeepAliveTimeout();
                }
            });
        }
    };

    window.onbeforeunload = function () {
        if (self.self.HasChanges()) {
            return "You have unsaved changes.";
        }
    }

    self.setKeepAliveTimeout();
}