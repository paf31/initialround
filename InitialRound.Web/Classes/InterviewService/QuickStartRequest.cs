using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class QuickStartRequest
    {
        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }

        public string EmailAddress { get; set; }

        public Guid QuestionSetID { get; set; }

        public int MinutesAllowed { get; set; }

        public bool SendInvitation { get; set; }
    }
}