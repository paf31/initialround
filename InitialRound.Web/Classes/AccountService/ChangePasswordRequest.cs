using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.AccountService
{
    public class ChangePasswordRequest
    {
        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}