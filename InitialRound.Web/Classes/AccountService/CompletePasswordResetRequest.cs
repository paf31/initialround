using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.AccountService
{
    public class CompletePasswordResetRequest
    {
        public string Token { get; set; }

        public string Username { get; set; }

        public string NewPassword { get; set; }
    }
}
