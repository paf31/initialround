using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class GetInterviewRequest
    {
        public string AuthToken { get; set; }

        public Guid InterviewID { get; set; }
    }
}