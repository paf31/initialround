using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.QuestionService
{
    public class DownloadInputRequest
    {
        public string AuthToken { get; set; }

        public Guid QuestionID { get; set; }
    }
}
