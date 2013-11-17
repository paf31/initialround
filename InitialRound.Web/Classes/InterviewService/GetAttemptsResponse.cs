using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InitialRound.BusinessLogic.Classes.Services;

namespace InitialRound.Web.Classes.InterviewService
{
    public class GetAttemptsResponse
    {
        public GetAttemptsResponseItem[] Attempts { get; set; }
    }
}