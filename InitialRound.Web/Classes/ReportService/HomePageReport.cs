using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.ReportService
{
    public class HomePageReport
    {
        public int NewInterviews { get; set; }

        public int PendingInterviews { get; set; }

        public int InterviewsInProgress { get; set; }

        public int InterviewsRequiringReview { get; set; }
    }
}