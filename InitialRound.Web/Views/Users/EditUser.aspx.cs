using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InitialRound.Web.Classes;
using E = InitialRound.Models.Schema.dbo;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic.Exceptions;
using System.Text.RegularExpressions;
using InitialRound.Models.Contexts;
using InitialRound.BusinessLogic.Helpers;

namespace InitialRound.Web.Views.Users
{
    public partial class EditUser : AdminPageBase
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