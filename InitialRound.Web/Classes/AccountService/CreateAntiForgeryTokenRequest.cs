using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.AccountService
{
    public class CreateAntiForgeryTokenRequest
    {
        public string AuthToken { get; set; }
    }
}
