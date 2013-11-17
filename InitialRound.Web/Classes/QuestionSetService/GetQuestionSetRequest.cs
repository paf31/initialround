using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.QuestionSetService
{
    public class GetQuestionSetRequest
    {
        public string AuthToken { get; set; }

        public Guid QuestionSetID { get; set; }
    }
}