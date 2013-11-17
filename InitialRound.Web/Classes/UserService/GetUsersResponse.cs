using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.UserService
{
    public class GetUsersResponse
    {
        public int TotalCount { get; set; }

        public GetUsersResponseItem[] Results { get; set; }
    }
}