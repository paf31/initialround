using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class GetAttemptDetailsRequest
    {
        public string AuthToken { get; set; }

        public Guid AttemptID { get; set; }
    }
}