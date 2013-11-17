///
///

using InitialRound.Models.Schema.dbo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.Helpers
{
    public static class TimeHelper
    {
        public static int CalculateSecondsRemaining(Interview interview)
        {
            int totalSeconds = interview.MinutesAllowed * 60;

            int secondsRemaining = totalSeconds;

            if (interview.StartedDate.HasValue)
            {
                double elapsedSeconds = (DateTime.UtcNow - interview.StartedDate.Value).TotalSeconds;

                secondsRemaining = (int)(totalSeconds - elapsedSeconds);
            }

            return secondsRemaining;
        }

        public static string FormatTime(int totalSeconds)
        {
            totalSeconds = Math.Max(0, totalSeconds);

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds / 60) % 60;
            int seconds = totalSeconds % 60;

            if (hours == 0)
            {
                return string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }
        }

        public static string CalculateTimeOffset(DateTime date1, DateTime date2)
        {
            return FormatTime((int)(date2 - date1).TotalSeconds);
        }
    }
}
