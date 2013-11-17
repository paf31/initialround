using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.UserService
{
    public class GetUserRequest
    {
        public string AuthToken { get; set; }

        public string Username { get; set; }
    }
}