using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.ApplicantService
{
    public class EditApplicantRequest
    {
        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }

        public Guid ApplicantID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }
    }
}