using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.InterviewService
{
    public class LoadInterviewResponse
    {
        public short StatusID { get; set; }

        public int MinutesAllowed { get; set; }

        public int NumberOfQuestions { get; set; }
    }
}
