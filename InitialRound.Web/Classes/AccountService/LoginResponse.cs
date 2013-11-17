using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.AccountService
{
    public class LoginResponse
    {
        public string AuthToken { get; set; }

        public int ExpirySeconds { get; set; }
    }
}