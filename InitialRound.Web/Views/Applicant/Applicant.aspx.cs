using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using E = InitialRound.Models.Schema.dbo;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.Web.Classes;
using InitialRound.BusinessLogic.Exceptions;
using System.Text.RegularExpressions;
using InitialRound.BusinessLogic;
using InitialRound.BusinessLogic.Helpers;
using InitialRound.Models.Contexts;

namespace InitialRound.Web.Views.Applicant
{
    public partial class Applicant : AuthenticatedPageBase
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