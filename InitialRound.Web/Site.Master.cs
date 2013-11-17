using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic;
using InitialRound.BusinessLogic.Classes;
using InitialRound.BusinessLogic.Helpers;

namespace InitialRound.Web
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        public AuthToken AuthToken { get; private set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            try
            {
                HttpCookie authTokenCookie = Request.Cookies[Constants.AuthToken];

                AuthToken authToken;

                if (authTokenCookie != null && !string.IsNullOrEmpty(authTokenCookie.Value) &&
                    UserController.ValidateSession(HttpUtility.UrlDecode(authTokenCookie.Value), out authToken))
                {
                    AuthToken = authToken;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, string.Empty);
            }
        }
    }
}
