using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InitialRound.Web.Classes;
using InitialRound.BusinessLogic.Exceptions;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic.Helpers;

namespace InitialRound.Web.Views.Users
{
    public partial class DeleteUser : AdminPageBase
    {
        protected override void UserAuthenticated()
        {
            base.UserAuthenticated();

            CreateAntiForgeryToken();
        }
    }
}