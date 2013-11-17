///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic.Entities;

namespace InitialRound.BusinessLogic.Helpers
{
    public class ExceptionHelper
    {
        public static void Log(Exception ex, string username)
        {
            try
            {
                ErrorLog newErrorLog = new ErrorLog(ex.ToString(), username);
                Common.Controllers.DataController.Insert(DataController.CreateServiceContext(), Constants.ErrorLog, newErrorLog);
            }
            catch (Exception)
            {
            }
        }
    }
}
