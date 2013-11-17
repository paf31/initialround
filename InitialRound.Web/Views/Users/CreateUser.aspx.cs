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
using System.Text.RegularExpressions;

namespace InitialRound.Web.Views.Users
{
    public partial class CreateUser : AdminPageBase
    {
        protected override void UserAuthenticated()
        {
            base.UserAuthenticated();

            if (!Page.IsPostBack)
            {
                CreateAntiForgeryToken();
            }
        }
    }
}