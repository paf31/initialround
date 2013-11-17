using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class GetInterviewsRequest
    {
        public string AuthToken { get; set; }

        public string Name { get; set; }

        public short? Status { get; set; }

        public Guid? ApplicantID { get; set; }

        public int StartAt { get; set; }
    }
}