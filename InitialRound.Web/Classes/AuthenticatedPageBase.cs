using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using InitialRound.BusinessLogic;
using InitialRound.BusinessLogic.Controllers;
using System.Web.Routing;
using InitialRound.BusinessLogic.Classes;

namespace InitialRound.Web.Classes
{
    public class AuthenticatedPageBase : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            if (AuthToken == null)
            {
                Response.Redirect("~/Login");
                Response.Close();
            }
            else
            {
                UserAuthenticated();
            }
        }

        protected virtual void UserAuthenticated()
        {
        }

        protected string AntiForgeryToken
        {
            get;
            set;
        }

        protected AuthToken AuthToken
        {
            get
            {
                return (Master as SiteMaster).AuthToken;
            }
        }

        protected void CreateAntiForgeryToken()
        {
            AntiForgeryToken = UserController.NewAntiForgeryToken(AuthToken.Username);
        }
    }
}