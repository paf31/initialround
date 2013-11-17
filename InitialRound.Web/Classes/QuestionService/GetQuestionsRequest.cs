using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InitialRound.BusinessLogic.Enums;

namespace InitialRound.Web.Classes.QuestionService
{
    public class GetQuestionsRequest
    {
        public string AuthToken { get; set; }

        public int StartAt { get; set; }

        public string Name { get; set; }
    }
}