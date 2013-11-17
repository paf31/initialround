using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InitialRound.BusinessLogic.Classes.Services;

namespace InitialRound.Web.Classes.InterviewService
{
    public class SubmitAnswerResponse
    {
        public RunTestsResult[] TestResults { get; set; }
    }
}