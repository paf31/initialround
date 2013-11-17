Applicant.prototype = new ViewModel();
Applicant.prototype.constructor = Applicant;

ApplicantInterviews.prototype = new ListViewModel();
ApplicantInterviews.prototype.constructor = ApplicantInterviews;

function ApplicantInterviews(applicantId) {
    var self = this;
    self.ListViewModel(this);

    self.applicantId = applicantId;

    self.Columns = [
        {
            Header: 'Status',
            Property: 'Status',
            Filter: {
                Property: 'Status',
                Type: 'select',
                Options: [
                    { Text: 'Any', Value: null },
                    { Text: 'Created', Value: 0 },
                    { Text: 'Invitation Sent', Value: 1 },
                    { Text: 'In Progress', Value: 2 },
                    { Text: 'Completed', Value: 3 },
                ]
            }
        },
        {
            Header: 'Last Updated By',
            Property: 'LastUpdatedBy',
            Filter: null
        },
        {
            Header: 'Last Updated On',
            Property: 'LastUpdatedDate',
            Filter: null
        },
        {
            Header: 'Created By',
            Property: 'CreatedBy',
            Filter: null
        },
        {
            Header: 'Created On',
            Property: 'CreatedDate',
            Filter: null
        }
    ];

    self.DeleteURITemplate = "/Interviews/Delete/";
    self.DetailsURITemplate = "/Interviews/Details/";

    self.serviceAddress = "/services/interviews/GetInterviews";

    self.Name = ko.observable(null);
    self.Status = ko.observable(null);

    self.PopulateQuery = function (query) {
        query.Name = self.Name();
        query.Status = self.Status();
        query.ApplicantID = self.applicantId;
    };

    self.Name.subscribe(self.LoadResults);
    self.Status.subscribe(self.LoadResults);

    self.LoadResults();
}

function Applicant(antiForgeryToken, applicantId) {
    var self = this;
    self.ViewModel(this);

    self.antiForgeryToken = antiForgeryToken;

    self.ApplicantID = ko.observable(applicantId);
    self.FirstName = ko.observable(null);
    self.LastName = ko.observable(null);
    self.EmailAddress = ko.observable(null);

    self.ApplicantInterviews = new ApplicantInterviews(applicantId);
    self.ApplicantInterviews.Busy.subscribe(self.Busy);

    self.Validate = function () {
        if (!self.FirstName() || self.FirstName() == '') {
            self.ErrorMessage("Please enter the applicant's first name.");
            return false;
        }

        if (!self.LastName() || self.LastName() == '') {
            self.ErrorMessage("Please enter the applicant's last name.");
            return false;
        }

        if (!self.EmailAddress() || self.EmailAddress() == '') {
            self.ErrorMessage("Please enter the applicant's email address.");
            return false;
        }

        var emailAddress = /\S+@\S+\.\S+/;

        if (self.EmailAddress() && !emailAddress.test(self.EmailAddress())) {
            self.ErrorMessage("Please enter a valid email address.");
            return false;
        } 

        return true;
    };

    self.LoadApplicant = function () {
        self.IncrTasks();
        $.ajax({
            url: "/services/applicants/GetApplicant",
            data: ko.toJSON({
                AuthToken: cookie("authtoken"),
                ApplicantID: self.ApplicantID()
            }),
            success: function (response) {
                self.DecrTasks();
                self.FirstName(response.FirstName);
                self.LastName(response.LastName);
                self.EmailAddress(response.EmailAddress);
                self.AddEventHandlers();
            }
        });
    }

    self.SaveCommand = function () {
        self.ErrorMessage(null);

        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/applicants/EditApplicant",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    ApplicantID: self.ApplicantID(),
                    FirstName: self.FirstName(),
                    LastName: self.LastName(),
                    EmailAddress: self.EmailAddress()
                }),
                success: function (response) {
                    self.DecrTasks();
                    self.HasChanges(false);
                }
            });
        }
    };

    self.CreateCommand = function () {
        self.ErrorMessage(null);

        if (self.Validate()) {
            self.IncrTasks();
            $.ajax({
                url: "/services/applicants/CreateApplicant",
                data: ko.toJSON({
                    AuthToken: cookie("authtoken"),
                    AntiForgeryToken: self.antiForgeryToken,
                    FirstName: self.FirstName(),
                    LastName: self.LastName(),
                    EmailAddress: self.EmailAddress()
                }),
                success: function (response) {
                    self.DecrTasks();
                    self.HasChanges(false);

                    window.location = '/Applicants/Details/' + response.ApplicantID;
                }
            });
        }
    };

    self.AddEventHandlers = function () {
        self.FirstName.subscribe(function () { self.HasChanges(true); });
        self.LastName.subscribe(function () { self.HasChanges(true); });
        self.EmailAddress.subscribe(function () { self.HasChanges(true); });
    };

    if (self.ApplicantID() != null) {
        self.LoadApplicant();
    } else {
        self.AddEventHandlers();
    }
}
