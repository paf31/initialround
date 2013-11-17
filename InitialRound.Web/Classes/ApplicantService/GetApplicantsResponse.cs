using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.ApplicantService
{
    public class GetApplicantsResponse
    {
        public int TotalCount { get; set; }

        public GetApplicantsResponseItem[] Results { get; set; }
    }
}