using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.InterviewService
{
    public class CreateAttemptTokenRequest
    {
        public string Token { get; set; }

        public Guid QuestionID { get; set; }
    }
}
