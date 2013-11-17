using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class CreateInterviewRequest
    {
        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }

        public Guid ApplicantID { get; set; }

        public int MinutesAllowed { get; set; }

        public bool UseQuestionSet { get; set; }

        public Guid? QuestionSetID { get; set; }

        public Guid[] QuestionIDs { get; set; }
    }
}