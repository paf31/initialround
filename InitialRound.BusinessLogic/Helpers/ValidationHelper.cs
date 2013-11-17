///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Properties;

namespace InitialRound.BusinessLogic.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidatePassword(string password)
        {
            string message = string.Format("Your password must be at least {0} characters long, and contain at least one upper case character, one lower case character and " +
                        "one numeric digit.", Settings.Default.MinimumPasswordLength);

            Common.Helpers.ValidationHelper.Assert(password != null && password.Length >= Settings.Default.MinimumPasswordLength &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit), message);
        }
    }
}
