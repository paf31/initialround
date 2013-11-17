using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InitialRound.BusinessLogic;
using InitialRound.BusinessLogic.Classes;
using InitialRound.BusinessLogic.Controllers;

namespace InitialRound.Web.Views.Interview
{
    public partial class Print : System.Web.UI.Page
    {
        public AuthToken AuthToken { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.OnInit(e);

            HttpCookie authTokenCookie = Request.Cookies[Constants.AuthToken];

            AuthToken authToken;

            if (authTokenCookie != null && !string.IsNullOrEmpty(authTokenCookie.Value) &&
                UserController.ValidateSession(HttpUtility.UrlDecode(authTokenCookie.Value), out authToken))
            {
                AuthToken = authToken;
            }
            else
            {
                Response.Redirect("~/Login");
                Response.Close();
            }
        }
    }
}