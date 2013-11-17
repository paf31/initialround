using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class DeleteInterviewRequest
    {
        public Guid InterviewID { get; set; }

        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }
    }
}