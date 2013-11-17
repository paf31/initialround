using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.QuestionSetService
{
    public class GetQuestionSetsRequest
    {
        public string AuthToken { get; set; }

        public string Name { get; set; }

        public int StartAt { get; set; }
    }
}