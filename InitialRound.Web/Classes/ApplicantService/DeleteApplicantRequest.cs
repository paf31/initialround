using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.ApplicantService
{
    public class DeleteApplicantRequest
    {
        public Guid ApplicantID { get; set; }

        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }
    }
}