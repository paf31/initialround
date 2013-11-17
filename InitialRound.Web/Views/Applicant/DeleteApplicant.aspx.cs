using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using E = InitialRound.Models.Schema.dbo;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.Web.Classes;
using InitialRound.BusinessLogic;
using InitialRound.Models.Contexts;
using InitialRound.BusinessLogic.Helpers;

namespace InitialRound.Web.Views.Applicant
{
    public partial class DeleteApplicant : AuthenticatedPageBase
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