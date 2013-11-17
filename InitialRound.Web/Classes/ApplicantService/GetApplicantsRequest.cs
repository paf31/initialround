using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.ApplicantService
{
    public class GetApplicantsRequest
    {
        public string AuthToken { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public int StartAt { get; set; }
    }
}