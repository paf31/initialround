using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using InitialRound.BusinessLogic;
using InitialRound.BusinessLogic.Controllers;
using System.Web.Routing;
using InitialRound.BusinessLogic.Classes;
using System.Net;

namespace InitialRound.Web.Classes
{
    public class AdminPageBase : AuthenticatedPageBase
    {
        protected override void UserAuthenticated()
        {
            if (!AuthToken.IsAdmin)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Response.Close();
            }

            base.UserAuthenticated();
        }
    }
}