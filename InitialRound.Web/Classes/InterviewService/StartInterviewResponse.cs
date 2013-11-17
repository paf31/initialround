using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class StartInterviewResponse
    {
        public int SecondsRemaining { get; set; }

        public InterviewQuestion[] Questions { get; set; }
    }
}