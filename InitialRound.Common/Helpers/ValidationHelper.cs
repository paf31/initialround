using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;
using InitialRound.Common.Exceptions;

namespace InitialRound.Common.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateEmailAddress(string emailAddress)
        {
            try
            {
                new MailAddress(emailAddress);
            }
            catch (FormatException)
            {
                throw new ValidationException("Email address format is invalid.");
            }
        }

        public static void ValidateRequiredField(string value, string fieldName)
        {
            AssertFalse(string.IsNullOrEmpty(value), string.Format("{0} is required.", fieldName));
        }

        public static void ValidateStringLength(string value, string fieldName, int maxLength)
        {
            Assert(value == null || value.Length <= maxLength, string.Format("{0} must be less than {1} characters long.", fieldName, maxLength));
        }

        public static void ValidateMinStringLength(string value, string fieldName, int minLength)
        {
            Assert(value == null || value.Length >= minLength, string.Format("{0} must be at least {1} characters long.", fieldName, minLength));
        }

        public static void ValidateUsername(string username)
        {
            Assert(username != null && username.Length >= 4 && username.All(char.IsLetterOrDigit), 
                "Your username must contain at least four characters and consist of only letters and digits.");
        }

        public static void Assert(bool b, string message)
        {
            if (!b)
            {
                throw new ValidationException(message);
            }
        }

        public static void AssertFalse(bool b, string message)
        {
            if (b)
            {
                throw new ValidationException(message);
            }
        }
    }
}
