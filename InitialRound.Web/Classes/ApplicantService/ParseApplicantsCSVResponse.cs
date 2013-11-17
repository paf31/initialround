using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.ApplicantService
{
    public class ParseApplicantsCSVResponse
    {
        public CSVApplicant[] Applicants { get; set; }
    }

    public class CSVApplicant
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }
    }
}
