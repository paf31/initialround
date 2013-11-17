///
///

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.Enums
{
    public enum InterviewStatus
    {
        [Description("Created")]
        Created = 0,
        [Description("Invitation Sent")]
        WaitingForApplicant = 1,
        [Description("In Progress")]
        InProgress = 2,
        [Description("Completed")]
        Completed = 3
    }
}
