using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.AccountService
{
    public class KeepAliveResponse
    {
        public string NewAuthToken { get; set; }

        public int ExpirySeconds { get; set; }
    }
}
