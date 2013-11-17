using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.QuestionService
{
    public class RunTestsRequest
    {
        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }
 
        public Guid QuestionID { get; set; }

        public string Output { get; set; }
    }
}
