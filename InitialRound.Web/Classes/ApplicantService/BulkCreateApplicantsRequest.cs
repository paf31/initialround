using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.ApplicantService
{
    public class BulkCreateApplicantsRequest
    {
        public string AntiForgeryToken { get; set; }

        public string AuthToken { get; set; }

        public CSVApplicant[] Applicants { get; set; }
    }
}
