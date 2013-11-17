using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Classes;
using InitialRound.BusinessLogic.Entities;
using InitialRound.Common.Classes;

namespace InitialRound.BusinessLogic
{
    public static class Constants
    {
        public static readonly string AuthToken = "authtoken";
        public static readonly string Expires = "expires";

        public static readonly string DefaultPartition = "default";

        public static readonly EntitySetName<ErrorLog> ErrorLog = new EntitySetName<ErrorLog>("ErrorLog");

        public static readonly string EmailQueue = "emails";

        public const string StripePlanName = "InitialRound";

        public const int MaxNameLength = 100;
        public const int MaxEmailAddressLength = 200;
        public const int MaxBlobLength = 20000;
        public const int MinUsernameLength = 4;
        public const int MaxUsernameLength = 50;
        public const int MaxPasswordLength = 20;
        public const int MaxPlanLength = 20;
        public const int MaxErrorMessageLength = 250;
        public const int MaxInterviewDuration = 120;
    }
}
