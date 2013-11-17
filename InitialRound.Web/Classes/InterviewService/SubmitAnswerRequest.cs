using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class SubmitAnswerRequest
    {
        public string Token { get; set; }

        public Guid ID { get; set; }

        public string Code { get; set; }

        public string Output { get; set; }

        public string AttemptToken { get; set; }
    }
}