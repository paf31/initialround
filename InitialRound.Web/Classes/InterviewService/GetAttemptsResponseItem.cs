using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Classes.Services;
using InitialRound.BusinessLogic.Enums;

namespace InitialRound.Web.Classes.InterviewService
{
    public class GetAttemptsResponseItem
    {
        public Guid AttemptID { get; set; }

        public string TimeOffset { get; set; }
    }
}
