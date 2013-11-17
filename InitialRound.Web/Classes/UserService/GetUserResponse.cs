using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.UserService
{
    public class GetUserResponse
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string CreatedDate { get; set; }

        public string LastUpdatedDate { get; set; }
    }
}