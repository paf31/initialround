using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.UserService
{
    public class GetUsersRequest
    {
        public string AuthToken { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public string Username { get; set; }

        public int StartAt { get; set; }
    }
}