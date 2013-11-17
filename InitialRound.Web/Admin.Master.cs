using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic;
using InitialRound.BusinessLogic.Classes;

namespace InitialRound.Web
{
    public partial class AdminMaster : System.Web.UI.MasterPage
    {
        public AuthToken AuthToken { get; private set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            HttpCookie authTokenCookie = Request.Cookies[Constants.AuthToken];

            AuthToken authToken;

            if (authTokenCookie != null && !string.IsNullOrEmpty(authTokenCookie.Value) &&
                UserController.ValidateSession(HttpUtility.UrlDecode(authTokenCookie.Value), out authToken))
            {
                AuthToken = authToken;
            }
        }
    }
}
