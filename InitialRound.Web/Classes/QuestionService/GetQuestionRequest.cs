using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.QuestionService
{
    public class GetQuestionRequest
    {
        public string AuthToken { get; set; }

        public Guid QuestionID { get; set; }
    }
}