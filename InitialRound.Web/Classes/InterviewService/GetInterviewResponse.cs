using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class GetInterviewResponse
    {
        public Guid ApplicantID { get; set; }

        public string ApplicantName { get; set; }

        public short Status { get; set; }

        public string StatusText { get; set; }

        public int MinutesAllowed { get; set; }

        public string LastUpdatedBy { get; set; }

        public string LastUpdatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public InterviewQuestionItem[] Questions { get; set; }

        public string TimeRemaining { get; set; }
    }
}