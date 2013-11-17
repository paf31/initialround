using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InitialRound.BusinessLogic;
using InitialRound.BusinessLogic.Controllers;

namespace InitialRound.Web.Views.Account
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                UserController.ClearAuthCookies(Request, Response);
            }
        }
    }
}