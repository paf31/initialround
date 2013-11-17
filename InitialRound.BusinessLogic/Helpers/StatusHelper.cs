///
///

using InitialRound.BusinessLogic.Enums;
using InitialRound.Models.Schema.dbo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.Helpers
{
    public static class StatusHelper
    {
        public static InterviewStatus GetInterviewStatus(Interview interview)
        {
            if (!interview.SentDate.HasValue)
            {
                return InterviewStatus.Created;
            }
            else if (!interview.StartedDate.HasValue)
            {
                return InterviewStatus.WaitingForApplicant;
            }
            else if (interview.StartedDate.Value.AddMinutes(interview.MinutesAllowed) >= DateTime.UtcNow)
            {
                return InterviewStatus.InProgress;
            }
            else
            {
                return InterviewStatus.Completed;
            }
        }
    }
}
