using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class GetAttemptDetailsResponse
    {
        public string Code { get; set; }

        public string Output { get; set; }

        public TestResultSummary[] Results { get; set; }
    }
}