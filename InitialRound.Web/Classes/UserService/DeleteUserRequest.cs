using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.UserService
{
    public class DeleteUserRequest
    {
        public string Username { get; set; }

        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }
    }
}