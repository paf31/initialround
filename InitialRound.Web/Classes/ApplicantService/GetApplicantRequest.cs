using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.ApplicantService
{
    public class GetApplicantRequest
    {
        public string AuthToken { get; set; }

        public Guid ApplicantID { get; set; }
    }
}