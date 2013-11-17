using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.UserService
{
    public class GetUsersResponseItem
    {
        public string Username { get; set; }

        public bool IsAdmin { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public string CreatedDate { get; set; }

        public string LastUpdatedDate { get; set; }
    }
}
