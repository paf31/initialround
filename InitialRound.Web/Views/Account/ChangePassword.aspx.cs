using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic.Helpers;
using InitialRound.BusinessLogic.Exceptions;
using InitialRound.BusinessLogic.Properties;
using InitialRound.BusinessLogic.Classes;
using InitialRound.Web.Classes;

namespace InitialRound.Web.Views.Account
{
    public partial class ChangePassword : AuthenticatedPageBase
    {
        protected override void UserAuthenticated()
        {
            if (!Page.IsPostBack)
            {
                CreateAntiForgeryToken();
            }
        }
    }
}