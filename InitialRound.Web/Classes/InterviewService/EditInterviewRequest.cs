using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class EditInterviewRequest
    {
        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }

        public int MinutesAllowed { get; set; }

        public Guid InterviewID { get; set; }

        public Guid[] QuestionIDs { get; set; }
    }
}